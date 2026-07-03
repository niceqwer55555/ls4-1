using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Vi - The Piltover Enforcer
    /// </summary>
    public class CharScriptVi : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Vi被动技能: Denting Blows - 攻击造成额外伤害
        }
    }
}