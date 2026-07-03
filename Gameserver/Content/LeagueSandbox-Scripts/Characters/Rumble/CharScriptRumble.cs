using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Rumble - The Boy Who Shotted Broil
    /// </summary>
    public class CharScriptRumble : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Rumble被动技能: Junkyard Titan - 过热系统
        }
    }
}