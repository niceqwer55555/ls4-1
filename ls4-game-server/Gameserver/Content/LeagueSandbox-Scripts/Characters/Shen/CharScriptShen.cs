using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Shen - The Eye of Twilight
    /// </summary>
    public class CharScriptShen : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Shen被动技能: Ki Strike - 技能获得额外伤害
        }
    }
}