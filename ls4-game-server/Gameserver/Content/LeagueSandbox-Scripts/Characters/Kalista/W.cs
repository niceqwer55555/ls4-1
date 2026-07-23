using System.Numerics;
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
    /// <summary>
    /// Kalista W - Sentinel
    /// Sends a sentinel to patrol an area, granting vision
    /// If the sentinel spots an enemy champion, it follows them for a few seconds
    /// Also: Kalista and her bound ally gain bonus damage when attacking the same target
    /// </summary>
    public class KalistaW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var distance = Vector2.Distance(owner.Position, targetPos);
            if (distance > 5000f)
            {
                targetPos = GetPointFromUnit(owner, 5000f);
            }

            AddParticle(owner, owner, "KalistaW_cas", owner.Position);

            // Spawn a sentinel minion at the target position
            Minion Sentinel = AddMinion(owner, "TestCubeRender", "TestCubeRender", targetPos, owner.Team, owner.SkinID, true, false);
            AddBuff("KalistaW", 6f, 1, spell, Sentinel, owner, false);

            // Apply the W passive buff to Kalista for the soul-bound damage
            AddBuff("KalistaWPassive", 25000f, 1, spell, owner, owner, true);
        }
    }
}
