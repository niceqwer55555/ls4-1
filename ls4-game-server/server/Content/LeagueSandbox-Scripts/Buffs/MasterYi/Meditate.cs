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
    internal class Meditate : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HEAL
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        float[] healthTick =
        {
            15f,
            25f,
            35f,
            45f,
            55f
        };

        float[] damageReduction =
        {
            0.40f,
            0.45f,
            0.50f,
            0.55f,
            0.60f
        };

        ObjAIBase owner;
        float tickTime;
        float elapsedTime;
        float trueHeal;
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
            var missingHealthBonus = healthTick[spellLevel] * ((owner.Stats.HealthPoints.Total - owner.Stats.CurrentHealth) / owner.Stats.HealthPoints.Total);
            var apBonus = owner.Stats.AbilityPower.Total * 0.15f;
            trueHeal = healthTick[spellLevel] + missingHealthBonus + apBonus;

            var newHealth = owner.Stats.CurrentHealth + trueHeal;
            owner.Stats.CurrentHealth = Math.Min(newHealth, owner.Stats.HealthPoints.Total);

            ApiEventManager.RemoveAllListenersForOwner(this);

            if (buffParticle != null)
            {
                buffParticle.SetToRemove();
            }
        }

        public void OnUpdate(float diff)
        {
            elapsedTime += diff;

            if (tickTime >= 500.0f)
            {
                var missingHealthBonus = healthTick[spellLevel] * ((owner.Stats.HealthPoints.Total - owner.Stats.CurrentHealth) / owner.Stats.HealthPoints.Total);
                var apBonus = owner.Stats.AbilityPower.Total * 0.15f;
                trueHeal = healthTick[spellLevel] + missingHealthBonus + apBonus;

                var newHealth = owner.Stats.CurrentHealth + trueHeal;
                owner.Stats.CurrentHealth = Math.Min(newHealth, owner.Stats.HealthPoints.Total);
                tickTime = 0;
            }

            tickTime += diff;
        }
    }
}
