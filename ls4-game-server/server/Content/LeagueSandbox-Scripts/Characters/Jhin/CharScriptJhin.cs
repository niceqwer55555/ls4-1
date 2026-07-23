using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Jhin - The Virtuoso
    /// </summary>
    public class CharScriptJhin : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Jhin被动技能: Dancing Grenades - 技能和攻击造成额外伤害
        }
    }
}