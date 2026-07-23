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
    /// Teemo E - Toxic Shot (passive)
    /// Toxic Shot is a passive ability triggered by auto-attacks.
    /// The actual on-hit damage is handled by ToxicShotAttack.cs.
    /// This script only manages the passive buff on Teemo himself.
    /// </summary>
    public class ToxicShot : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = false,
            NotSingleTargetSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            // Passive is active as long as the spell is learned
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, System.Numerics.Vector2 start, System.Numerics.Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
        }

        public void OnSpellPostCast(Spell spell)
        {
            // Passive skill - no active cast behavior
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
