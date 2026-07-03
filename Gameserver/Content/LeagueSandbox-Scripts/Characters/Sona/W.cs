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
    /// Sona W - Aria of Perseverance
    /// 治疗自身和一个附近受伤的友军
    /// </summary>
    public class SonaW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 治疗: 35/50/65/80/95 (+30% AP)
            float[] healAmount = { 35f, 50f, 65f, 80f, 95f };
            float ap = owner.Stats.AbilityPower.Total * 0.3f;
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 治疗自己
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + heal, owner.Stats.HealthPoints.Total);

            // 治疗附近受伤友军
            AttackableUnit lowestHealthAlly = null;
            float lowestHealth = float.MaxValue;

            foreach (var unit in GetUnitsInRange(owner.Position, 1000f, true))
            {
                if (unit.Team == owner.Team && unit is Champion && unit != owner)
                {
                    if (unit.Stats.CurrentHealth < lowestHealth)
                    {
                        lowestHealth = unit.Stats.CurrentHealth;
                        lowestHealthAlly = unit;
                    }
                }
            }

            if (lowestHealthAlly != null)
            {
                lowestHealthAlly.Stats.CurrentHealth = Math.Min(lowestHealthAlly.Stats.CurrentHealth + heal, lowestHealthAlly.Stats.HealthPoints.Total);
                // 护盾: 25/50/75/100/125 (+30% AP)
                float[] shieldAmount = { 25f, 50f, 75f, 100f, 125f };
                float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;
                AddBuff("Shield", 1.5f, 1, spell, lowestHealthAlly, owner);
            }

            AddParticle(owner, owner, "SonaW_cas", owner.Position);
        }
    }
}
