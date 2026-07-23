using System;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using System.Linq;

namespace Spells
{
    /// <summary>
    /// Lissandra R - Frozen Tomb
    /// On enemy: 100/150/200 (+70% AP) damage + stun 1.5s
    /// On self: Heal for 100/150/200 (+70% AP) + untargetable 2.5s + slow nearby enemies
    /// </summary>
    public class LissandraR : ISpellScript
    {
        ObjAIBase Lissandra;
        AttackableUnit Target;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            Lissandra = spell.CastInfo.Owner as Champion;
            Target = Lissandra.Spells[3].CastInfo.Targets[0].Unit;

            if (Lissandra == Target)
            {
                // Self-cast: heal + become untargetable
                AddBuff("LissandraRSelf", 2.5f, 1, spell, Lissandra, Lissandra);
                // Heal: 100/150/200 (+70% AP)
                float[] baseHeal = { 100f, 150f, 200f };
                float heal = baseHeal[spell.CastInfo.SpellLevel - 1] + (float)(Lissandra.Stats.AbilityPower.Total * 0.7);
                Lissandra.Stats.CurrentHealth = Math.Min(Lissandra.Stats.CurrentHealth + heal, Lissandra.Stats.HealthPoints.Total);
                // Slow nearby enemies
                foreach (var unit in GetUnitsInRange(Lissandra.Position, 450f, true))
                {
                    if (unit.Team != Lissandra.Team)
                    {
                        AddBuff("LissandraR", 1.5f, 1, spell, unit, Lissandra);
                    }
                }
            }
            else
            {
                // Enemy cast: damage + stun
                SpellCast(Lissandra, 3, SpellSlotType.ExtraSlots, false, Target, Vector2.Zero);
            }
        }
    }

    /// <summary>
    /// Lissandra R - Enemy cast
    /// </summary>
    public class LissandraREnemy : ISpellScript
    {
        ObjAIBase Lissandra;
        AttackableUnit Target;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            Lissandra = spell.CastInfo.Owner as Champion;
            Target = Lissandra.Spells[3].CastInfo.Targets[0].Unit;
            // Damage: 100/150/200 (+70% AP)
            float[] baseDamage = { 100f, 150f, 200f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + (float)(Lissandra.Stats.AbilityPower.Total * 0.7);
            Target.TakeDamage(Lissandra, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddBuff("LissandraREnemy2", 1.5f, 1, spell, Target, Lissandra);
            AddParticleTarget(Lissandra, Target, "Lissandra_Base_R_tar", Target, 1f);
            // Slow nearby enemies
            foreach (var unit in GetUnitsInRange(Target.Position, 450f, true))
            {
                if (unit.Team != Lissandra.Team && unit != Target)
                {
                    AddBuff("LissandraR", 1.5f, 1, spell, unit, Lissandra);
                }
            }
        }
    }
}

