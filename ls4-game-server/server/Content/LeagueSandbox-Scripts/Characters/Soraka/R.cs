using System;
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
    /// Soraka R - Wish
    /// 治疗所有友军，不管他们在地图的哪个位置
    /// </summary>
    public class SorakaR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 治疗: 200/320/440 (+0.7 AP)
            float[] healAmount = { 200f, 320f, 440f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 获取所有友军并治疗
            foreach (var unit in GetUnitsInRange(owner.Position, 25000f, true))
            {
                if (unit.Team == owner.Team && unit is Champion)
                {
                    unit.Stats.CurrentHealth = Math.Min(unit.Stats.CurrentHealth + heal, unit.Stats.HealthPoints.Total);
                }
            }

            AddParticle(owner, owner, "SorakaR_cas", owner.Position);
        }
    }
}
