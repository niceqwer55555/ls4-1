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
    /// Vladimir Q - Transfusion
    /// 抽取目标血液造成伤害并治疗自己
    /// </summary>
    public class VladimirQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;

            // 魔法伤害: 70/100/130/160/190 (+0.45 AP)
            float[] baseDamage = { 70f, 100f, 130f, 160f, 190f };
            float ap = owner.Stats.AbilityPower.Total * 0.45f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 治疗: 35/45/55/65/75 (+0.25 AP)
            float[] healAmount = { 35f, 45f, 55f, 65f, 75f };
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 0.25f;
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + heal, owner.Stats.HealthPoints.Total);

            AddParticle(owner, owner, "VladimirQ_cas", target.Position);
        }
    }
}
