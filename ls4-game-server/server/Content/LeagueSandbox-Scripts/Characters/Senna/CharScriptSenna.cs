using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Senna - The Redeemer
    /// </summary>
    public class CharScriptSenna : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Senna被动技能: Thorne of the void - 击杀获得攻击力和穿甲
        }
    }
}