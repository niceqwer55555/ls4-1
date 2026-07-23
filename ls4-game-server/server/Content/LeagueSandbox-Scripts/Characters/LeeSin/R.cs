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
    /// LeeSin R - Dragon's Rage
    /// 踢退目标敌人，对目标及其撞到的敌人造成伤害
    /// </summary>
    public class DragonsRage : ISpellScript
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

            // 伤害公式: 200/400/600 + 2.0 AD
            float[] baseDamage = { 200f, 400f, 600f };
            float totalAd = owner.Stats.AttackDamage.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + totalAd * 2.0f;

            // 对目标造成伤害
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 击退目标
            var direction = Vector2.Normalize(target.Position - owner.Position);
            var knockbackTarget = target.Position + direction * 800f;
            TeleportTo((ObjAIBase)target, knockbackTarget.X, knockbackTarget.Y);

            // 眩晕目标1秒
            AddBuff("Stun", 1f, 1, spell, target, owner);

            // 对目标撞到的敌人造成一半伤害
            float secondaryDamage = damage * 0.5f;
            foreach (var unit in GetUnitsInRange(target.Position, 300f, true))
            {
                if (unit.Team != owner.Team && unit != target)
                {
                    unit.TakeDamage(owner, secondaryDamage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticleTarget(owner, target, "Dragon'sRage_cas", target);
        }
    }
}
