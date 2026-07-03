using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Quinn - The Demacian Wings
    /// </summary>
    public class CharScriptQuinn : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Quinn被动技能: Harrier - 技能标记敌人
        }
    }
}