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
    /// Knocks back enemies and heals allies for 90/150/210 (+50% AP) per half-second for 3 seconds
    /// </summary>
    public class JannaR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false,
            NotSingleTargetSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // Heal per tick: 90/150/210 (+50% AP)
            float[] healAmount = { 90f, 150f, 210f };
            float ap = (float)owner.Stats.AbilityPower.Total * 0.5f;
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // Knock back enemies
            foreach (var unit in GetUnitsInRange(owner.Position, 725f, true))
            {
                if (unit.Team != owner.Team)
                {
                    AddBuff("JannaR", 0.5f, 1, spell, unit, owner);
                }
            }

            // Heal allies over 3 seconds (6 ticks of 0.5s)
            for (int i = 0; i < 6; i++)
            {
                var delay = 0.5f * (i + 1);
                CreateTimer(delay, () =>
                {
                    if (owner != null && !owner.IsDead)
                    {
                        foreach (var unit in GetUnitsInRange(owner.Position, 725f, true))
                        {
                            if (unit.Team == owner.Team)
                            {
                                unit.Stats.CurrentHealth = Math.Min(unit.Stats.CurrentHealth + heal, unit.Stats.HealthPoints.Total);
                            }
                        }
                    }
                });
            }

            AddParticle(owner, owner, "JannaR_cas", owner.Position);
        }
    }
}
