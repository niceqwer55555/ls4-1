using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Ezreal - The Prodigal Explorer
    /// </summary>
    public class CharScriptEzreal : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Ezreal被动技能: Rising Spell Force - 攻击叠层提升攻速
        }
    }
}