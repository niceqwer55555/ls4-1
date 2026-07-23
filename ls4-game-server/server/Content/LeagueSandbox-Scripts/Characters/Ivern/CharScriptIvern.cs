using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Ivern - The Green Father
    /// </summary>
    public class CharScriptIvern : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Ivern被动技能: Friend of the Forest - 不能攻击野怪，用技能种树
        }
    }
}