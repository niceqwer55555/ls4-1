using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Heimerdinger - The Revered Inventor
    /// </summary>
    public class CharScriptHeimerdinger : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Heimerdinger被动技能: Hextech Affinity - 技能获得冷却缩减
        }
    }
}