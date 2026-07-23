using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Annie - The Dark Child
    /// </summary>
    public class CharScriptAnnie : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Annie被动技能: Pyromania - 技能叠层，下一次技能晕眩
        }
    }
}