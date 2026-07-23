using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AI脚本: Smolder - 幼小蓝焰龙
    /// 被动技能: 龙息 (Dragon Breath)
    /// </summary>
    public class CharScriptSmolder : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Smolder被动: 超远距离技能伤害
        }
    }
}