using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Soraka - The Starchild
    /// </summary>
    public class CharScriptSoraka : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Soraka被动技能: Salvation - 移动和技能帮助友军
        }
    }
}