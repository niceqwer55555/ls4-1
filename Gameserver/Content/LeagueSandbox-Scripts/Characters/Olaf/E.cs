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
    /// Olaf E - Reckless Swing
    /// 对目标造成真实伤害，但自身也会受到反噬
    /// </summary>
    public class RecklessSwing : ISpellScript
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

            // 真实伤害: 100/160/220/280/340
            float[] baseDamage = { 100f, 160f, 220f, 280f, 340f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1];

            var target = spell.CastInfo.Targets[0].Unit;
            if (target != null && target.Team != owner.Team)
            {
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);

                // 反噬伤害（造成伤害的30%）
                float selfDamage = damage * 0.3f;
                owner.TakeDamage(owner, selfDamage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELLPERSIST, false);
            }

            AddParticle(owner, owner, "RecklessSwing_cas", owner.Position);
        }
    }
}
