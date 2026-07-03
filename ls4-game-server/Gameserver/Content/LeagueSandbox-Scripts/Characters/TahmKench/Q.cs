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
    /// TahmKench Q - Tongue Lash
    /// 投掷舌头伤害敌人并减速
    /// </summary>
    public class TahmKenchQ : ISpellScript
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

            // 物理伤害: 80/130/180/230/280 (+0.7 bonus AD)
            float[] baseDamage = { 80f, 130f, 180f, 230f, 280f };
            float bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + bonusAd * 0.7f;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 减速: 30/40/50/60/70%
            float[] slowAmount = { 0.30f, 0.40f, 0.50f, 0.60f, 0.70f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];
            AddBuff("Slow", 2f, 1, spell, target, owner);

            AddParticle(owner, owner, "TahmKenchQ_cas", target.Position);
        }
    }
}
