using System.Numerics;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;
using System.Linq;

namespace Spells
{
    /// <summary>
    /// Jax E - Counter Strike
    /// Dodges all basic attacks for 2 seconds, then stuns nearby enemies for 1s
    /// Deals 50/75/100/125/150 (+80% bonus AD) physical damage
    /// First cast: start dodging, second cast: end early
    /// </summary>
    public class JaxCounterStrike : ISpellScript
    {
        ObjAIBase Jax;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            Jax = spell.CastInfo.Owner as Champion;
            if (!Jax.HasBuff("JaxCounterStrike"))
            {
                spell.SetCooldown(1f, true);
                AddBuff("JaxCounterStrike", 2f, 1, spell, Jax, Jax, false);
            }
            else
            {
                // End early
                RemoveBuff(Jax, "JaxCounterStrike");
                StunAndDamage(spell);
            }
        }

        /// <summary>
        /// Stun nearby enemies and deal damage
        /// </summary>
        private void StunAndDamage(Spell spell)
        {
            // Damage: 50/75/100/125/150 (+80% bonus AD)
            float[] baseDamage = { 50f, 75f, 100f, 125f, 150f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + Jax.Stats.AttackDamage.FlatBonus * 0.8f;

            foreach (var unit in GetUnitsInRange(Jax.Position, 375f, true))
            {
                if (unit.Team != Jax.Team)
                {
                    unit.TakeDamage(Jax, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Stun", 1.0f, 1, spell, unit, Jax);
                }
            }
            AddParticle(Jax, null, "Jax_Base_E_cas.troy", Jax.Position);
        }
    }
}
