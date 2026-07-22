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
    /// Nunu Q - Consume
    /// 吞食小型敌人造成伤害并治疗自己
    /// </summary>
    public class NunuQ : ISpellScript
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

            // 真实伤害: 80/130/180/230/280 (+0.9 AP)
            float[] baseDamage = { 80f, 130f, 180f, 230f, 280f };
            float ap = owner.Stats.AbilityPower.Total * 0.9f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 治疗: 60/90/120/150/180 (+0.5 AP)
            float[] healAmount = { 60f, 90f, 120f, 150f, 180f };
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 0.5f;
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + heal, owner.Stats.HealthPoints.Total);
        }
    }
}
