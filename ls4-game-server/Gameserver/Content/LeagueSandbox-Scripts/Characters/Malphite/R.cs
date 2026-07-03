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
    public class MalphiteR : ISpellScript
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

            float[] baseDamage = { 200f, 300f, 400f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            foreach (var unit in GetUnitsInRange(targetPos, 350f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Knockup", 0.75f, 1, spell, unit, owner);
                }
            }

            var trueCoords = GetPointFromUnit(owner, 1000f);
            ForceMovement(owner, "Spell3", trueCoords, 1200, 0, 0, 0);

            AddParticle(owner, owner, "MalphiteR_cas", targetPos);
        }
    }
}