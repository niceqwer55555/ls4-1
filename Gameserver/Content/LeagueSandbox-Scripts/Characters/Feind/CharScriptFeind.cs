using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Feind - Minion
    /// </summary>
    public class CharScriptFeind : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Feind 被动技能: 无
        }
    }
}