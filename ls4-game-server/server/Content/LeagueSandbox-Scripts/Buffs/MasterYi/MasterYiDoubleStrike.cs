using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.API;
using System.Numerics;

namespace Buffs
{
    /// <summary>
    /// MasterYi Double Strike buff - next attack deals 50% AD bonus physical damage.
    /// Consumed on next auto-attack.
    /// </summary>
    internal class MasterYiDoubleStrike : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        Buff thisBuff;
        ObjAIBase owner;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            thisBuff = buff;
            owner = ownerSpell.CastInfo.Owner;
            // Listen for next auto-attack to trigger double strike
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, false);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            ApiEventManager.OnLaunchAttack.RemoveListener(this);
        }

        /// <summary>
        /// On next attack: cast the DoubleStrike extra spell for bonus 50% AD damage.
        /// </summary>
        public void OnLaunchAttack(Spell spell)
        {
            if (thisBuff != null && !thisBuff.Elapsed())
            {
                // Remove this buff (consumed)
                ApiEventManager.OnLaunchAttack.RemoveListener(this);
                spell.CastInfo.Owner.RemoveBuff(thisBuff);

                // Skip the normal auto-attack and cast Double Strike instead
                spell.CastInfo.Owner.SkipNextAutoAttack();
                SpellCast(spell.CastInfo.Owner, 0, SpellSlotType.ExtraSlots, false, spell.CastInfo.Owner.TargetUnit, Vector2.Zero);
            }
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
