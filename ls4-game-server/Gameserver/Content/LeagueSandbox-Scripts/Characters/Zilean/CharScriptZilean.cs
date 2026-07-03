using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Zilean - The Chronokeeper
    /// </summary>
    public class CharScriptZilean : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Zilean被动技能: Time in a Bottle - 获得额外经验
        }
    }
}