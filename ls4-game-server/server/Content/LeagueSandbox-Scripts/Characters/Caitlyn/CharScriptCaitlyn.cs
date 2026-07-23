using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Caitlyn - The Sheriff of Piltover
    /// </summary>
    public class CharScriptCaitlyn : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Caitlyn被动技能: Headshot - 攻击叠层后暴击
        }
    }
}