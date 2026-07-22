using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Corki - The Daring Bombardier
    /// </summary>
    public class CharScriptCorki : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Corki被动技能: Hextech Munitions - 大招变为核弹
        }
    }
}