using System.Numerics;
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

namespace Spells
{
    /// <summary>
    /// Jax R - Grandmaster's Might
    /// Passive: Every 3rd attack deals 100/140/180 (+70% AP) bonus magic damage
    /// Active: Gain 25/45/65 (+30% bonus AD) armor and MR for 8 seconds
    /// </summary>
    public class JaxRelentlessAssault : ISpellScript
    {
        ObjAIBase Jax;
        Spell RelentlessAssault;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            RelentlessAssault = spell;
            Jax = spell.CastInfo.Owner as Champion;
            ApiEventManager.OnLevelUpSpell.AddListener(this, spell, OnLevelUpSpell, true);
        }

        public void OnLevelUpSpell(Spell spell)
        {
            ApiEventManager.OnLaunchAttack.AddListener(this, Jax, OnLaunchAttack, false);
        }

        public void OnLaunchAttack(Spell spell)
        {
            AddBuff("JaxRelentlessAttack", 3f, 1, RelentlessAssault, Jax, Jax);
        }

        public void OnSpellCast(Spell spell)
        {
            AddBuff("JaxRelentlessAssault", 8f, 1, spell, Jax, Jax, false);
        }
    }

    /// <summary>
    /// Jax R Passive - third hit bonus damage
    /// Damage: 100/140/180 (+70% AP)
    /// </summary>
    public class JaxRelentlessAttack : ISpellScript
    {
        float Damage;
        ObjAIBase Jax;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellCast(Spell spell)
        {
            Jax = spell.CastInfo.Owner as Champion;
            // Damage: 100/140/180 (+70% AP)
            float[] baseDamage = { 100f, 140f, 180f };
            Damage = baseDamage[Jax.Spells[3].CastInfo.SpellLevel - 1] + (float)(Jax.Stats.AbilityPower.Total * 0.7);
            Jax.TargetUnit.TakeDamage(Jax, Damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            AddParticleTarget(Jax, Jax.TargetUnit, "RelentlessAssault_tar.troy", Jax.TargetUnit, 1f);
        }
    }
}
