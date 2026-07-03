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
    internal class MasterYiDoubleStrike : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };


        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        Particle pbuff;
        Particle pbuff2;
        Buff thisBuff;
        ObjAIBase owner;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            thisBuff = buff;
            owner = ownerSpell.CastInfo.Owner;
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, false);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(pbuff);
            RemoveParticle(pbuff2);
            ApiEventManager.OnLaunchAttack.RemoveListener(this);
        }

        public void OnLaunchAttack(Spell spell)
        {
            if (thisBuff != null && !thisBuff.Elapsed())
            {
                ApiEventManager.OnLaunchAttack.RemoveListener(this);
                spell.CastInfo.Owner.RemoveBuff(thisBuff);
                spell.CastInfo.Owner.SkipNextAutoAttack();
                SpellCast(spell.CastInfo.Owner, 0, SpellSlotType.ExtraSlots, false, spell.CastInfo.Owner.TargetUnit, Vector2.Zero);
            }
        }
        public void OnUpdate(float diff)
        {
        }
    }
}