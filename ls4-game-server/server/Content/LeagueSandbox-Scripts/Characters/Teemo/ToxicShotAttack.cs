using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    /// <summary>
    /// Teemo E - Toxic Shot on-hit effect.
    /// Applies poison on auto-attack hits.
    /// </summary>
    public class ToxicShotAttack : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = false,
            NotSingleTargetSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            if (spell.CastInfo.Targets == null || spell.CastInfo.Targets.Count == 0)
            {
                return;
            }

            var target = spell.CastInfo.Targets[0]?.Unit;
            if (target == null)
            {
                return;
            }

            // Get ToxicShot (E) spell level from the owner
            var toxicShotSpell = owner.GetSpell("ToxicShot");
            int spellLevel = toxicShotSpell?.CastInfo.SpellLevel ?? 1;
            if (spellLevel <= 0)
            {
                return;
            }

            float ap = owner.Stats.AbilityPower.Total;

            // On-hit magic damage: 10/20/30/40/50 (+30% AP)
            float[] initialDamage = { 10f, 20f, 30f, 40f, 50f };
            float initial = initialDamage[spellLevel - 1] + (ap * 0.3f);
            target.TakeDamage(owner, initial, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLPERSIST, false);

            // Poison DoT: 6/12/18/24/30 (+10% AP) per second for 4 seconds
            AddBuff("ToxicShot", 4f, 1, toxicShotSpell, target, owner);

            // Add poison particle effect
            AddParticleTarget(owner, target, "ToxicShot_tar.troy", target);
        }
    }
}
