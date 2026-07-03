using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Lucian - The Purifier
    /// </summary>
    public class CharScriptLucian : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Lucian被动技能: Lightslinger - 攻击两次，第二次更强大
        }
    }
}