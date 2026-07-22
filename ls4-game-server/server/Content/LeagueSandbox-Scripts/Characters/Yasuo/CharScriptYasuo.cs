using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Yasuo - The Unforgiven
    /// </summary>
    public class CharScriptYasuo : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Yasuo被动技能: Way of the Wanderer - 获得暴击率和护盾
        }
    }
}