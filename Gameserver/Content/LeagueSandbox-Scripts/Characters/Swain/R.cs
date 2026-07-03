using System;
using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;

namespace Spells
{
    /// <summary>
    /// Swain R - Demonic Ascension
    /// 变身为恶魔形态，造成范围伤害并治疗自己
    /// </summary>
    public class SwainR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 每秒魔法伤害: 100/150/200 (+0.4 AP)
            float[] dpsDamage = { 100f, 150f, 200f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float dps = dpsDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 治疗效果: 150/250/350 (+0.5 AP)
            float[] healAmount = { 150f, 250f, 350f };
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 0.5f;

            foreach (var unit in GetUnitsInRange(owner.Position, 600f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, dps, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 恢复生命
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + heal, owner.Stats.HealthPoints.Total);

            AddParticle(owner, owner, "SwainR_cas", owner.Position);
        }
    }
}
