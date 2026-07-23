using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Warwick - The Uncaged Wrath of Zaun
    /// </summary>
    public class CharScriptWarwick : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Warwick被动技能: Eternal Hunger - 攻击回复生命值
        }
    }
}