using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Kled - The Cantankerous Cavalier
    /// </summary>
    public class CharScriptKled : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Kled被动技能: Skaarl the Cowardly Lizard - 坐骑承受伤害
        }
    }
}