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
    /// Karthus E - Death Defied
    /// 死亡抗拒
    /// </summary>
    public class KarthusE : ISpellScript
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

            // 魔法伤害: 30/50/70/90/110 (+0.2 AP)
            float[] baseDamage = { 30f, 50f, 70f, 90f, 110f };
            float ap = owner.Stats.AbilityPower.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap * 0.2f;

            foreach (var unit in GetUnitsInRange(owner.Position, 550f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 自我治疗
            float healAmount = damage * 0.5f;
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + healAmount, owner.Stats.HealthPoints.Total);

            AddParticle(owner, owner, "KarthusE_cas", owner.Position);
        }
    }
}
