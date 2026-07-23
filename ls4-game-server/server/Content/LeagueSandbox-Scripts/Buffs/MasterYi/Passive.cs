using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Buffs
{
    /// <summary>
    /// MasterYi Passive - Double Strike tracker.
    /// Stacks up to 3. At 3 stacks, next attack triggers Double Strike.
    /// </summary>
    internal class MasterYiPassive : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.DAMAGE,
            BuffAddType = BuffAddType.STACKS_AND_RENEWS,
            MaxStacks = 3
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        Spell spell;
        ObjAIBase ownerUnit;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            ownerUnit = ownerSpell.CastInfo.Owner;
            spell = ownerSpell;

            // At max stacks (3), trigger Double Strike
            if (buff.StackCount >= 3)
            {
                AddBuff("MasterYiDoubleStrike", 3.0f, 1, spell, ownerUnit, ownerUnit);
                // Remove all stacks to reset
                buff.DeactivateBuff();
            }
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
