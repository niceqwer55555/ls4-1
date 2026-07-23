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
    public class MoveQuick : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var spellLevel = spell.CastInfo.SpellLevel;

            // Remove passive buff before applying active buff to avoid stacking
            if (owner.HasBuff("MoveQuickPassive"))
            {
                owner.RemoveBuffsWithName("MoveQuickPassive");
            }

            // Active move speed bonus: 20/28/36/44/52% for 3 seconds
            AddBuff("MoveQuickActive", 3f, 1, spell, owner, owner);

            // Add particle effect
            AddParticleTarget(owner, owner, "MoveQuick_buf.troy", owner);
        }
    }
}
