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
    public class BlindingDart : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var target = spell.CastInfo.Targets[0].Unit;

            // Magic damage: 80/125/170/215/260 (+80% AP)
            float[] baseDamage = { 80f, 125f, 170f, 215f, 260f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // Blind duration: 1.5/1.75/2.0/2.25/2.5s
            float[] blindDuration = { 1.5f, 1.75f, 2.0f, 2.25f, 2.5f };
            float blindTime = blindDuration[spell.CastInfo.SpellLevel - 1];

            // Apply damage
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // Apply blind buff
            AddBuff("Blind", blindTime, 1, spell, target, owner);

            // Add hit effect particle
            AddParticleTarget(owner, target, "BlindShot_tar.troy", target);
        }
    }
}
