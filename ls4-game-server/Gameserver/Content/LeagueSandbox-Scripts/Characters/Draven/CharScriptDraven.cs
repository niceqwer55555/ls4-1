using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Draven - The Glorious Executioner
    /// </summary>
    public class CharScriptDraven : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Draven被动技能: League of Draven - 获得额外的金币
        }
    }
}