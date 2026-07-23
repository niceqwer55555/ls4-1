using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Sona - Maven of the Strings
    /// </summary>
    public class CharScriptSona : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Sona被动技能: Power Chord - 技能叠层强化下次攻击
        }
    }
}