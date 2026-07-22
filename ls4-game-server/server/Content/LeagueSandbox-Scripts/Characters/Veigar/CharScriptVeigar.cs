using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Veigar - The Tiny Master of Evil
    /// </summary>
    public class CharScriptVeigar : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Veigar被动技能: Equilibrium - 技能击杀获得额外AP
        }
    }
}