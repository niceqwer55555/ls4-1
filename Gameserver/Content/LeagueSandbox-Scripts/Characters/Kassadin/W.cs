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
    /// Kassadin W - Nether Blade
    /// 虚空之刃 - 下一次攻击造成额外魔法伤害并回复法力
    /// </summary>
    public class NetherBlade : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 伤害公式: 40/65/90/115/140 + 0.7 AP
            float[] baseDamage = { 40f, 65f, 90f, 115f, 140f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 对目标造成额外魔法伤害
            var target = spell.CastInfo.Targets[0].Unit;
            if (target != null)
            {
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            }

            // 回复已损失法力值的4/5/6/7/8%
            float manaPercent = 0.04f + (spell.CastInfo.SpellLevel - 1) * 0.01f;
            float manaRestore = owner.Stats.CurrentMana * manaPercent;
            owner.Stats.CurrentMana += manaRestore;

            AddParticle(owner, owner, "NetherBlade_cas", owner.Position);
        }
    }
}
