using System;
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
    /// Nidalee E (Human) - Primal Surge
    /// 治疗友军并提供攻击速度加成
    /// </summary>
    public class PrimalSurge : ISpellScript
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

            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;

            // 治疗公式: 50/85/120/155/190 + 0.7 AP
            float[] baseHeal = { 50f, 85f, 120f, 155f, 190f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float heal = baseHeal[spell.CastInfo.SpellLevel - 1] + ap;

            // 治疗目标
            if (target.Team == owner.Team)
            {
                target.Stats.CurrentHealth = Math.Min(target.Stats.CurrentHealth + heal, target.Stats.HealthPoints.Total);
            }

            // 攻击速度加成: 20/30/40/50/60%
            float[] attackSpeedBonus = { 0.20f, 0.30f, 0.40f, 0.50f, 0.60f };
            float attackSpeed = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 添加攻击速度buff
            AddBuff("AttackSpeedBuff", 5f, 1, spell, target, owner);

            AddParticleTarget(owner, target, "PrimalSurge_cas", target);
        }
    }
}
