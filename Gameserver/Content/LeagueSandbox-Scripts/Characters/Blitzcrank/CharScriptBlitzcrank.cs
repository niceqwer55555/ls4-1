using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Blitzcrank - The Great Steam Golem
    /// </summary>
    public class CharScriptBlitzcrank : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Blitzcrank被动技能: Mana Barrier - 低蓝量时获得护盾
        }
    }
}