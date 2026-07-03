using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// TahmKench - The River King
    /// </summary>
    public class CharScriptTahmKench : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // TahmKench被动技能: An Acquired Taste - 技能命中叠层
        }
    }
}