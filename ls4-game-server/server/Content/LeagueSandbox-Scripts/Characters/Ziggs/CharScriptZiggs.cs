using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Ziggs - The Hexplosives Expert
    /// </summary>
    public class CharScriptZiggs : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Ziggs被动技能: Short Fuse - 技能造成额外伤害
        }
    }
}