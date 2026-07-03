using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Jinx - The Loose Cannon
    /// </summary>
    public class CharScriptJinx : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Jinx被动技能: Get Excited! - 击杀获得移速和攻速加成
        }
    }
}