using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;

namespace Buffs
{
    internal class MoveQuickActive : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HASTE,
            BuffAddType = BuffAddType.STACKS_AND_OVERLAPS,
            IsHidden = false
        };

        Particle p;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            float[] activeMoveSpeed = { 0.20f, 0.28f, 0.36f, 0.44f, 0.52f };
            float activeSpeed = activeMoveSpeed[ownerSpell.CastInfo.SpellLevel - 1];

            StatsModifier.MoveSpeed.PercentBonus = activeSpeed;
            unit.AddStatModifier(StatsModifier);

            p = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "MoveQuick_buf.troy", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(p);
            unit.RemoveStatModifier(StatsModifier);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}




