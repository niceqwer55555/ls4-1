using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Darius - The Hand of Noxus
    /// </summary>
    public class CharScriptDarius : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Darius被动技能: Hemorrhage - 技能和攻击造成出血
        }
    }
}