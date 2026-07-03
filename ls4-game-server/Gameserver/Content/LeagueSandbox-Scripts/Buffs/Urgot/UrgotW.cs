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
    /// Urgot W - Command: Fearless
    /// 获得额外护盾
    /// </summary>
    internal class UrgotW : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HASTE,
            BuffAddType = BuffAddType.STACKS_AND_OVERLAPS,
            IsHidden = false
        };

        Particle shield;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // 护盾�? 60/90/120/150/180 + 0.4 AD
            float[] shieldAmount = { 60f, 90f, 120f, 150f, 180f };
            float ad = ownerSpell.CastInfo.Owner.Stats.AttackDamage.Total * 0.4f;
            float shieldValue = shieldAmount[ownerSpell.CastInfo.SpellLevel - 1] + ad;

            StatsModifier.HealthPoints.FlatBonus = shieldValue;
            unit.AddStatModifier(StatsModifier);

            shield = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Urgot_W_Shield", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(shield);
            unit.RemoveStatModifier(StatsModifier);
        }
    }
}




