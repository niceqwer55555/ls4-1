using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Taliyah - The Stoneweaver
    /// </summary>
    public class CharScriptTaliyah : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Taliyah被动技能: Unraveled Earth - 地形交互
        }
    }
}