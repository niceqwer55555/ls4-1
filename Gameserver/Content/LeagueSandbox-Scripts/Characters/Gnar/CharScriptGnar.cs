using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Gnar - The Yordle Protoe
    /// </summary>
    public class CharScriptGnar : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Gnar被动技能: Rage Gene - 移动和攻击积攒怒气，变身后获得额外属性
        }
    }
}