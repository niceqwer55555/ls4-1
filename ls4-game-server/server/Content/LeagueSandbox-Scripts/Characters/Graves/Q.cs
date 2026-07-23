using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Graves Q - Scatter Shot
    /// 向前锥形发射4颗弹丸，每颗造成物理伤害
    /// </summary>
    public class GravesQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 伤害公式: 55/70/85/100/115 + 0.75 bonus AD
            float[] baseDamage = { 55f, 70f, 85f, 100f, 115f };
            float bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + bonusAd * 0.75f;

            // 向前锥形发射
            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var direction = Vector2.Normalize(targetPos - owner.Position);
            foreach (var unit in GetUnitsInRange(owner.Position, 1000f, true))
            {
                if (unit.Team != owner.Team)
                {
                    // 检查是否在锥形范围内
                    var toUnit = Vector2.Normalize(unit.Position - owner.Position);
                    var dot = direction.X * toUnit.X + direction.Y * toUnit.Y;
                    if (dot > 0.5f)
                    {
                        unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    }
                }
            }

            AddParticle(owner, owner, "GravesQ_cas", owner.Position);
        }
    }
}
