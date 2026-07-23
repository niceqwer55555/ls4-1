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
    public class ForcePulse : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            float[] baseDamage = { 60f, 90f, 120f, 150f, 180f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            float slowAmount = 0.3f + (spell.CastInfo.SpellLevel - 1) * 0.1f;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var direction = Vector2.Normalize(targetPos - owner.Position);

            foreach (var unit in GetUnitsInRange(owner.Position, 400f, true))
            {
                if (unit.Team != owner.Team)
                {
                    var toUnit = Vector2.Normalize(unit.Position - owner.Position);
                    var dot = direction.X * toUnit.X + direction.Y * toUnit.Y;
                    if (dot > 0.1f)
                    {
                        unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                        AddBuff("Slow", 2f, 1, spell, unit, owner, false);
                    }
                }
            }

            AddParticle(owner, owner, "ForcePulse_cas", owner.Position);
        }
    }
}