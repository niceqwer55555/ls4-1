using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;

namespace Buffs
{
    /// <summary>
    /// Teemo W Active - Move Quick active buff.
    /// Grants 20/28/36/44/52% bonus move speed for 3 seconds.
    /// When expired, re-applies the passive buff.
    /// </summary>
    internal class MoveQuickActive : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HASTE,
            BuffAddType = BuffAddType.REPLACE_EXISTING,
            IsHidden = false
        };

        Particle p;
        Spell ownerSpell;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            this.ownerSpell = ownerSpell;

            float[] activeMoveSpeed = { 0.20f, 0.28f, 0.36f, 0.44f, 0.52f };
            float activeSpeed = activeMoveSpeed[ownerSpell.CastInfo.SpellLevel - 1];

            StatsModifier.MoveSpeed.PercentBonus = activeSpeed;
            unit.AddStatModifier(StatsModifier);

            p = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "MoveQuick_buf.troy", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.RemoveStatModifier(StatsModifier);
            RemoveParticle(p);

            // Re-apply passive buff when active expires
            var owner = ownerSpell.CastInfo.Owner;
            if (!owner.HasBuff("MoveQuickPassive"))
            {
                AddBuff("MoveQuickPassive", 1f, 1, ownerSpell, owner, owner, true);
            }
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
