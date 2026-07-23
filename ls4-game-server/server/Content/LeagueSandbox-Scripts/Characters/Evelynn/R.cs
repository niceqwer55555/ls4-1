using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class EvelynnR : ISpellScript
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

            float[] damagePercent = { 0.15f, 0.2f, 0.25f };
            float[] slow = { 0.3f, 0.5f, 0.7f };

            int heroCount = 0;
            foreach (var unit in GetUnitsInRange(owner.Position, 600f, true))
            {
                if (unit.Team != owner.Team && unit is Champion)
                {
                    heroCount++;
                    float damage = unit.Stats.HealthPoints.Total * damagePercent[spell.CastInfo.SpellLevel - 1];
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }

            float[] shieldAmount = { 150f, 225f, 300f };
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] * heroCount;
            if (shield > 0)
            {
                AddBuff("Shield", 3f, 1, spell, owner, owner);
            }

            AddParticle(owner, owner, "EvelynnR_cas", owner.Position);
        }
    }
}