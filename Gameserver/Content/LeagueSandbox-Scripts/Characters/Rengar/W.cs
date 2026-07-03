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
    /// Rengar W - Battle Roar
    /// 怒吼对周围敌人造成魔法伤害并获得治疗
    /// </summary>
    public class RengarW : ISpellScript
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

            // 魔法伤害: 50/80/110/140/170 (+0.8 AP)
            float[] baseDamage = { 50f, 80f, 110f, 140f, 170f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            foreach (var unit in GetUnitsInRange(owner.Position, 450f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 治疗效果: 30/40/50/60/70 (+0.5 AP)
            float[] healAmount = { 30f, 40f, 50f, 60f, 70f };
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 0.5f;
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + heal, owner.Stats.HealthPoints.Total);

            AddParticle(owner, owner, "RengarW_cas", owner.Position);
        }
    }
}
