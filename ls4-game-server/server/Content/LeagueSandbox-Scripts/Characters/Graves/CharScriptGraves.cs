using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Graves - The Outlaw
    /// </summary>
    public class CharScriptGraves : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Graves被动技能: New Destiny - 获得额外散弹枪弹药
        }
    }
}