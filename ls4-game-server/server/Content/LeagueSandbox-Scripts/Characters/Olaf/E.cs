using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class RecklessSwing : ISpellScript
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

            float[] baseDamage = { 100f, 150f, 200f, 250f, 300f };
            float ad = owner.Stats.AttackDamage.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target != null && target.Team != owner.Team)
            {
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);
                float selfDamage = damage * 0.3f;
                owner.TakeDamage(owner, selfDamage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELLPERSIST, false);

                TeleportTo(owner, target.Position.X, target.Position.Y);
            }

            AddParticle(owner, owner, "RecklessSwing_cas", owner.Position);
        }
    }
}