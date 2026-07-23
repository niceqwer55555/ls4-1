using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    /// <summary>
    /// Teemo W - Move Quick
    /// Passive: +10/14/18/22/26% move speed (always active while learned).
    /// Active: +20/28/36/44/52% move speed for 3 seconds.
    /// </summary>
    public class MoveQuick : ISpellScript
    {
        private ObjAIBase _owner;

        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            _owner = owner;
            // Apply passive move speed buff when W is learned
            AddBuff("MoveQuickPassive", 1f, 1, spell, owner, owner, true);
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
            // Remove passive buff when W is unlearned
            RemoveBuff(owner, "MoveQuickPassive");
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, System.Numerics.Vector2 start, System.Numerics.Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
        }

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;

            // Remove passive buff before applying active buff
            if (owner.HasBuff("MoveQuickPassive"))
            {
                owner.RemoveBuffsWithName("MoveQuickPassive");
            }

            // Active move speed bonus: 20/28/36/44/52% for 3 seconds
            AddBuff("MoveQuickActive", 3f, 1, spell, owner, owner);

            // Add particle effect
            AddParticleTarget(owner, owner, "MoveQuick_buf.troy", owner);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
