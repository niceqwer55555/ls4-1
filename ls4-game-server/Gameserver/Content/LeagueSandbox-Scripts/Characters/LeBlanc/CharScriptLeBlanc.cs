using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// LeBlanc - The Deceiver
    /// </summary>
    public class CharScriptLeBlanc : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // LeBlanc被动技能: Mirror Image - 生命值低时制造分身
        }
    }
}