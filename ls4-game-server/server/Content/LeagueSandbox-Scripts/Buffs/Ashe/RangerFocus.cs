using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Buffs
{
    /// <summary>
    /// Ashe Q - Ranger's Focus
    /// 增加攻击速度，持�?�?    /// </summary>
    internal class RangerFocus : IBuffGameScript
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
            // 攻击速度加成: 25/35/45/55/65%
            float[] attackSpeedBonus = { 0.25f, 0.35f, 0.45f, 0.55f, 0.65f };
            float attackSpeed = attackSpeedBonus[ownerSpell.CastInfo.SpellLevel - 1];

            StatsModifier.AttackSpeed.PercentBonus = attackSpeed;
            unit.AddStatModifier(StatsModifier);

            p = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Rangerfocus_buf.troy", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(p);
            unit.RemoveStatModifier(StatsModifier);
        }
    }
}




