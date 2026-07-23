using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Cassiopeia - The Serpent's Embrace
    /// </summary>
    public class CharScriptCassiopeia : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Cassiopeia被动技能: Aspect of the Serpent - 技能命中叠层，提升伤害
        }
    }
}