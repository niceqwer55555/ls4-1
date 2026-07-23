using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// MasterYi R - Highlander
    /// Grants bonus move speed, attack speed, slow immunity.
    /// Kills extend the buff duration and reset Q/W/E cooldowns.
    /// </summary>
    public class Highlander : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            // Duration: 10/15/20 seconds
            var level = spell.CastInfo.SpellLevel;
            float duration = 10f + (level - 1) * 5f;
            AddBuff("Highlander", duration, 1, spell, owner, owner);
        }

        public void OnSpellPostCast(Spell spell)
        {
        }
    }
}
