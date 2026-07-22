using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Maokai - The Twisted Treant
    /// </summary>
    public class CharScriptMaokai : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Maokai被动技能: Sap Magic - 受到攻击时回复
        }
    }
}