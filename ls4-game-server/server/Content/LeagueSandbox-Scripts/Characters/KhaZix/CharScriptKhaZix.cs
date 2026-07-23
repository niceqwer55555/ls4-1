using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// KhaZix - The Voidreaver
    /// </summary>
    public class CharScriptKhaZix : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // KhaZix被动技能: Unseen Threat - 隐身时下次攻击造成额外伤害
        }
    }
}