using System;
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
    /// Janna R - Monsoon
    /// 击退周围敌人并治疗友军
    /// </summary>
    public class JannaR : ISpellScript
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

            // 每0.5秒治疗: 35/55/75 (+0.175 AP)
            float[] healAmount = { 35f, 55f, 75f };
            float ap = owner.Stats.AbilityPower.Total * 0.175f;
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 对敌人造成伤害
            foreach (var unit in GetUnitsInRange(owner.Position, 725f, true))
            {
                if (unit.Team != owner.Team)
                {
                    // 击退效果
                    AddBuff("JannaR", 0.5f, 1, spell, unit, owner);
                }
                else if (unit.Team == owner.Team && unit is Champion)
                {
                    // 治疗友军
                    unit.Stats.CurrentHealth = Math.Min(unit.Stats.CurrentHealth + heal, unit.Stats.HealthPoints.Total);
                }
            }

            AddParticle(owner, owner, "JannaR_cas", owner.Position);
        }
    }
}
