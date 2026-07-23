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
    public class AkaliR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;

            float[] baseDamage = { 100f, 150f, 200f };
            float ad = owner.Stats.AttackDamage.Total * 0.65f;
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            float missingHealth = target.Stats.HealthPoints.Total - target.Stats.CurrentHealth;
            float ampDamage = baseDamage[spell.CastInfo.SpellLevel - 1] * 0.5f + missingHealth * 0.25f;
            target.TakeDamage(owner, ampDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            TeleportTo(owner, target.Position.X, target.Position.Y);

            AddParticle(owner, owner, "AkaliR_cas", owner.Position);
        }
    }
}