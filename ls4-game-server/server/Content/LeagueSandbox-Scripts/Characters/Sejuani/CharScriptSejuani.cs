using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Sejuani - The Winter's Claw
    /// </summary>
    public class CharScriptSejuani : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Sejuani被动技能: Frost Breath - 技能标记敌人
        }
    }
}