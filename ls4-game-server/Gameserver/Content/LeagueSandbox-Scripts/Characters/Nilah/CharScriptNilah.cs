using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AI脚本: Nilah - 无尽之刃
    /// 被动技能: 战场狂热 (Joy)
    /// </summary>
    public class CharScriptNilah : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Nilah被动: 获得队友的经验值和治疗效果
        }
    }
}