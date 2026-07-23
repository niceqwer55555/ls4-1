using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using System.Numerics;
using System;

namespace Spells
{
    /// <summary>
    /// Alistar E - Triumphant Roar
    /// Heals self and nearby allied units for 60/90/120/150/180 (+20% AP)
    /// </summary>
    public class TriumphantRoar : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false
        };

        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            // Heal: 60/90/120/150/180 (+20% AP)
            float[] baseHeal = { 60f, 90f, 120f, 150f, 180f };
            float ap = owner.Stats.AbilityPower.Total * 0.2f;
            float healAmount = baseHeal[spell.CastInfo.SpellLevel - 1] + ap;

            // Heal self
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + healAmount, owner.Stats.HealthPoints.Total);

            // Heal nearby allies
            foreach (var unit in GetUnitsInRange(owner.Position, 575f, true))
            {
                if (unit.Team == owner.Team && unit != owner)
                {
                    unit.Stats.CurrentHealth = Math.Min(unit.Stats.CurrentHealth + healAmount, unit.Stats.HealthPoints.Total);
                }
            }

            AddParticleTarget(owner, owner, "TriumphantRoar_cas.troy", owner, lifetime: 1f);
        }
    }
}
