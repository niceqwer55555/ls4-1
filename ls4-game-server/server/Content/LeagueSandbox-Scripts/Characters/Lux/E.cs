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
    /// Lux E - Lucent Singularity
    /// 发送扭曲之光到目标区域，减速敌人
    /// 5秒后或可再次激活时爆炸，造成伤害
    /// </summary>
    public class LucentSingularity : ISpellScript
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

            // 减速: 20/24/28/32/36%
            float[] slowAmount = { 0.20f, 0.24f, 0.28f, 0.32f, 0.36f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 减速区域内的敌人
            foreach (var unit in GetUnitsInRange(targetPos, 350f, true))
            {
                if (unit.Team != owner.Team)
                {
                    AddBuff("Slow", 5f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "LucentSingularity_cas", targetPos);
        }

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 伤害公式: 60/105/150/195/240 + 0.6 AP
            float[] baseDamage = { 60f, 105f, 150f, 195f, 240f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 5秒后爆炸对区域内的敌人造成伤害
            foreach (var unit in GetUnitsInRange(targetPos, 350f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticle(owner, owner, "LucentSingularity_exp", targetPos);
        }
    }
}
