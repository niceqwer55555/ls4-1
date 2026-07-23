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
    public class AkaliE : ISpellScript
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

            float[] baseDamage = { 21f, 42f, 63f, 84f, 105f };
            float ad = owner.Stats.AttackDamage.Total * 0.3f;
            float ap = owner.Stats.AbilityPower.Total * 0.33f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad + ap;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target != null)
            {
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            }

            var trueCoords = GetPointFromUnit(owner, -825f);
            ForceMovement(owner, "Spell3", trueCoords, 1000, 0, 0, 0);

            AddParticle(owner, owner, "AkaliE_cas", owner.Position);
        }
    }
}