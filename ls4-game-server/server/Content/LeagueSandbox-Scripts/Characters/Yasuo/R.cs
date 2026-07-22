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
    /// Yasuo R - Last Breath
    /// 闪烁到浮空敌人身边，造成物理伤害并延长浮空时间
    /// </summary>
    public class LastBreath : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;

            // 伤害公式: 200/400/600 + 1.5 bonus AD
            float[] baseDamage = { 200f, 400f, 600f };
            float totalAd = owner.Stats.AttackDamage.Total;
            float baseAd = 60f; // Approximate Yasuo base AD
            float bonusAd = totalAd - baseAd;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + bonusAd * 1.5f;

            // 位移到目标位置
            TeleportTo(owner, target.Position.X, target.Position.Y);

            // 对目标造成伤害
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 对范围内浮空敌人造成伤害并延长浮空时间
            foreach (var unit in GetUnitsInRange(target.Position, 300f, true))
            {
                if (unit.Team != owner.Team && unit != target)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 延长浮空时间
                    AddBuff("Airborne", 1f, 1, spell, unit, owner);
                }
            }

            // 获得最大层数的Flow
            AddBuff("YasuoFlow", 5f, 1, spell, owner, owner);

            AddParticleTarget(owner, target, "LastBreath_cas", target);
        }
    }
}
