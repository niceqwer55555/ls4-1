using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Urgot - The Dreadnought
    /// </summary>
    public class CharScriptUrgot : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Urgot被动技能: Heartbeat - 获得额外护甲和魔抗
        }
    }
}