using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Orianna - The Clockwork Countess
    /// </summary>
    public class CharScriptOrianna : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Orianna被动技能: Clockwork Windup - 攻击造成额外伤害
        }
    }
}