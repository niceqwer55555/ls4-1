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
    /// Shaco E - Two-Shiv Poison
    /// 投掷匕首造成魔法伤害并减速
    /// </summary>
    public class ShacoE : ISpellScript
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

            // 魔法伤害: 50/90/130/170/210 (+1.0 AP) (+1.0 Bonus AD)
            float[] baseDamage = { 50f, 90f, 130f, 170f, 210f };
            float ap = owner.Stats.AbilityPower.Total;
            float bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap + bonusAd;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 减速: 10/15/20/25/30%
            float[] slowAmount = { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];
            AddBuff("Slow", 2f, 1, spell, target, owner);
        }
    }
}
