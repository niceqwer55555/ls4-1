using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Illaoi - The Kraken Priestess
    /// </summary>
    public class CharScriptIllaoi : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Illaoi被动技能: Vessel Creator - 技能召唤触手
        }
    }
}