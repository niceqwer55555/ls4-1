using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AurelionSol - The Star Forger
    /// </summary>
    public class CharScriptAurelionSol : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // AurelionSol被动技能: Center of the Universe - 环绕的星星造成伤害
        }
    }
}