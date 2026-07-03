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
    /// Kassadin E - Force Pulse
    /// 能量脉冲 - 对锥形区域内敌人造成魔法伤害并减速
    /// </summary>
    public class ForcePulse : ISpellScript
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

            // 伤害公式: 80/105/130/155/180 + 0.7 AP
            float[] baseDamage = { 80f, 105f, 130f, 155f, 180f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 减速: 50/60/70/80/90% 持续1秒
            float slowAmount = 0.5f + (spell.CastInfo.SpellLevel - 1) * 0.1f;

            // 锥形范围攻击
            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var direction = Vector2.Normalize(targetPos - owner.Position);

            foreach (var unit in GetUnitsInRange(owner.Position, 400f, true))
            {
                if (unit.Team != owner.Team)
                {
                    var toUnit = Vector2.Normalize(unit.Position - owner.Position);
                    var dot = direction.X * toUnit.X + direction.Y * toUnit.Y;
                    if (dot > 0.5f)
                    {
                        unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                        AddBuff("Slow", 1f, 1, spell, unit, owner);
                    }
                }
            }

            AddParticle(owner, owner, "ForcePulse_cas", owner.Position);
        }
    }
}
