using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Viktor - The Machine of Progress
    /// </summary>
    public class CharScriptViktor : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Viktor被动技能: Glorious Evolution - 技能可以升级
        }
    }
}