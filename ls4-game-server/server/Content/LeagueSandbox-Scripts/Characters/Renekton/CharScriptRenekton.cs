using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Renekton - The Butcher of the Sands
    /// </summary>
    public class CharScriptRenekton : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Renekton被动技能: Reign of Anger - 获得怒气提升伤害
        }
    }
}