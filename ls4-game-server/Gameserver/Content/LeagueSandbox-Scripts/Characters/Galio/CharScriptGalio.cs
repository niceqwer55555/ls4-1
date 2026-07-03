using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Galio - The Colossus
    /// </summary>
    public class CharScriptGalio : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Galio被动技能: Colossal Smash - 技能造成额外伤害
        }
    }
}