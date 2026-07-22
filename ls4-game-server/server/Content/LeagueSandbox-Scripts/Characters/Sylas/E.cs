using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using System.Numerics;

namespace Spells
{
    public class SylasE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            if (owner == null) return;

            float[] baseDamage = { 60f, 90f, 120f, 150f, 180f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + (owner as Champion).Stats.AbilityPower.Total * 0.4f;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var trueCoords = GetPointFromUnit(owner, 475f);
            TeleportTo(owner, trueCoords.X, trueCoords.Y);

            foreach (var unit in GetUnitsInRange(owner.Position, 350f, true))
            {
                if (spell.SpellData.IsValidTarget(owner, unit))
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }
        }
    }
}