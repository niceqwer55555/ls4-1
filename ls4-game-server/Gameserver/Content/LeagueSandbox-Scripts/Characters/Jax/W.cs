using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    /// <summary>
    /// Jax W - Empower
    /// Empowers next attack dealing 40/75/110/145/180 (+60% AP) bonus magic damage
    /// </summary>
    public class JaxEmpowerTwo : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            owner.CancelAutoAttack(true);
            AddBuff("JaxEmpowerTwo", 5f, 1, spell, owner, owner, false);
        }
    }

    /// <summary>
    /// Jax W - Empower Attack (auto attack replacement)
    /// </summary>
    public class JaxEmpowerAttack : ISpellScript
    {
        float Damage;
        ObjAIBase Jax;
        AttackableUnit Target;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Target = target;
            Jax = owner = spell.CastInfo.Owner as Champion;
        }

        public void OnSpellCast(Spell spell)
        {
            // Damage: 40/75/110/145/180 (+60% AP)
            float[] baseDamage = { 40f, 75f, 110f, 145f, 180f };
            Damage = baseDamage[Jax.Spells[1].CastInfo.SpellLevel - 1] + (float)(Jax.Stats.AbilityPower.Total * 0.6);
            Target.TakeDamage(Jax, Damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            AddParticleTarget(Jax, Target, "EmpowerTwo_hit.troy", Target, 1f);
            AddParticleTarget(Jax, Target, "EmpowerTwoHit_tar.troy", Target, 1f);
        }
    }
}
