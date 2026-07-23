using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Thresh - The Chain Warden
    /// </summary>
    public class CharScriptThresh : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Thresh被动技能: Damnation - 击杀获得灵魂
        }
    }
}