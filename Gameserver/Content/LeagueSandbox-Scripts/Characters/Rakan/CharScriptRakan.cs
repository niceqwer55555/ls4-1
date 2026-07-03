using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Rakan - The Charmer
    /// </summary>
    public class CharScriptRakan : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Rakan被动技能: Fancy Footwork - 移动获得护盾
        }
    }
}