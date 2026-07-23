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
    /// Varus E - Hail of Arrows
    /// 对目标区域发射箭雨，造成物理伤害并污染地面4秒
    /// 污染地面的敌人减速50%且治疗效果降低50%
    /// </summary>
    public class VarusE : ISpellScript
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

            // 物理伤害: 65/105/145/185/225 (+0.6 bonus AD)
            float[] baseDamage = { 65f, 105f, 145f, 185f, 225f };
            float bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + bonusAd * 0.6f;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对范围内的敌人造成伤害
            foreach (var unit in GetUnitsInRange(targetPos, 300f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 减速: 25/30/35/40/45%
                    float[] slowAmount = { 0.25f, 0.30f, 0.35f, 0.40f, 0.45f };
                    float slow = slowAmount[spell.CastInfo.SpellLevel - 1];
                    AddBuff("HailOfArrows", 4f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "VarusE_cas", targetPos);
        }
    }
}
