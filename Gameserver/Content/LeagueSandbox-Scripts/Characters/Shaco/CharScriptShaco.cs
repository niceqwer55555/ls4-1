using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Shaco - The Demon Jester
    /// </summary>
    public class CharScriptShaco : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Shaco被动技能: Backstab - 背刺造成额外伤害
        }
    }
}