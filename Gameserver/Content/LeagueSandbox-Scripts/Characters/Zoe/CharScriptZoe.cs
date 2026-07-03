using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Zoe - The Aspect of Twilight
    /// </summary>
    public class CharScriptZoe : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Zoe被动技能: More Sparkles! - 技能获得额外伤害
        }
    }
}