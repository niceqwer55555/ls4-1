using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Udyr - The Spirit Guard
    /// </summary>
    public class CharScriptUdyr : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Udyr被动技能: Spirit Guard - 获得额外属性
        }
    }
}