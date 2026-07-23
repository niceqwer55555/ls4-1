using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Sett - The Boss
    /// </summary>
    public class CharScriptSett : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Sett被动技能: Pit Grit - 攻击造成额外伤害
        }
    }
}