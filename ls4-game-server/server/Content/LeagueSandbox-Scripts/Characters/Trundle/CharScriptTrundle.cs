using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Trundle - The Cursed King
    /// </summary>
    public class CharScriptTrundle : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Trundle被动技能: King's Tribute - 击杀回复生命值
        }
    }
}