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
    /// Diana E - Lunar Rush
    /// 变成复仇之月的活体化身，突进至单个敌人身边，并造成魔法伤害
    /// 对被施加了月光效果的敌人使用时，会重置冷却时间
    /// </summary>
    public class LunarRush : ISpellScript
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

            // 伤害公式: 100/160/220 + 0.6 AP
            float[] baseDamage = { 100f, 160f, 220f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 突进到目标位置
            TeleportTo(owner, target.Position.X, target.Position.Y);

            // 对目标造成伤害
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 清除月光效果
            // AddBuff("Moonlight", ...) - 移除debuff

            AddParticleTarget(owner, target, "LunarRush_cas", target);
        }
    }
}
