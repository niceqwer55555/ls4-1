using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Nautilus - The Titan of the Depths
    /// </summary>
    public class CharScriptNautilus : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Nautilus被动技能: Staggering Blow - 攻击造成额外伤害和眩晕
        }
    }
}