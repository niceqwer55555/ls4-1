using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Velkoz - The Eye of the Void
    /// </summary>
    public class CharScriptVelkoz : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Velkoz被动技能: Organic Decay - 技能标记敌人
        }
    }
}