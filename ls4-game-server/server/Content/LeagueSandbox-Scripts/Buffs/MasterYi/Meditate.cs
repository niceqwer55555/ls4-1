using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using System;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerLib.GameObjects.AttackableUnits;

namespace Buffs
{
    /// <summary>
    /// MasterYi W - Meditate buff
    /// Heals over time and reduces damage taken.
    /// </summary>
    internal class Meditate : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HEAL
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        // Heal per tick: 30/50/70/90/110 (+30% AP)
        float[] healthTick =
        {
            30f,
            50f,
            70f,
            90f,
            110f
        };

        // Damage reduction after first 0.5s: 50/55/60/65/70%
        float[] damageReduction =
        {
            0.50f,
            0.55f,
            0.60f,
            0.65f,
            0.70f
        };

        ObjAIBase owner;
        float tickTime;
        float elapsedTime;
        int spellLevel;
        Particle buffParticle;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner;
            spellLevel = ownerSpell.CastInfo.SpellLevel - 1;
            tickTime = 0f;
            elapsedTime = 0f;

            ApiEventManager.OnPreTakeDamage.AddListener(this, unit, PreTakeDamage, false);

            buffParticle = AddParticleTarget(unit, unit, "masteryi_base_w_buf", unit, 4.0f, flags: 0);
        }

        /// <summary>
        /// Reduce incoming damage. First 0.5s = 90% reduction, after that = 50/55/60/65/70%.
        /// </summary>
        public void PreTakeDamage(DamageData dmg)
        {
            float reduction = damageReduction[spellLevel];
            if (elapsedTime < 500.0f)
            {
                reduction = 0.90f;
            }
            dmg.PostMitigationDamage *= (1f - reduction);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            ApiEventManager.RemoveAllListenersForOwner(this);

            if (buffParticle != null)
            {
                buffParticle.SetToRemove();
            }
        }

        public void OnUpdate(float diff)
        {
            elapsedTime += diff;
            tickTime += diff;

            if (tickTime >= 500.0f)
            {
                // Heal: base + missing health bonus + AP ratio
                var missingHealthRatio = (owner.Stats.HealthPoints.Total - owner.Stats.CurrentHealth) / owner.Stats.HealthPoints.Total;
                var missingHealthBonus = healthTick[spellLevel] * missingHealthRatio;
                var apBonus = owner.Stats.AbilityPower.Total * 0.3f;
                var trueHeal = healthTick[spellLevel] + missingHealthBonus + apBonus;

                var newHealth = owner.Stats.CurrentHealth + trueHeal;
                owner.Stats.CurrentHealth = Math.Min(newHealth, owner.Stats.HealthPoints.Total);
                tickTime = 0f;
            }
        }
    }
}
