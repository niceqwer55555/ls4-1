using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;

namespace Spells
{
    /// <summary>
    /// Corki W - Valkyrie
    /// Dashes to target location, leaving a fire trail that deals damage over time
    /// </summary>
    public class CorkiW : ISpellScript
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

            var trueCoords = GetPointFromUnit(owner, 600f);
            ForceMovement(owner, "Spell3", trueCoords, 1000, 0, 0, 0);
            AddParticle(owner, owner, "CorkiW_cas", owner.Position);
            // Leave fire trail buff
            AddBuff("CorkiW", 2f, 1, spell, owner, owner);
        }
    }
}
