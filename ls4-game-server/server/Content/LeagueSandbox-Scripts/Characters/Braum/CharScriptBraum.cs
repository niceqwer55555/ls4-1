using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Braum - The Shield of Valoran
    /// </summary>
    public class CharScriptBraum : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Braum被动技能: Concussive Stikes - 标记敌人获得易损效果
        }
    }
}