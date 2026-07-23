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
    /// <summary>
    /// Gnar E - Hop
    /// Jumps to target location, gains movement speed on landing
    /// Damage is dealt if Mega Gnar or if landing on a unit
    /// </summary>
    public class GnarE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            AddBuff("GnarE", 3f, 1, spell, owner, owner);
            var trueCoords = GetPointFromUnit(owner, 475f);
            ForceMovement(owner, "Spell3", trueCoords, 1200, 0, 0, 0);
            AddParticle(owner, owner, "GnarE_cas", owner.Position);
        }
    }
}
