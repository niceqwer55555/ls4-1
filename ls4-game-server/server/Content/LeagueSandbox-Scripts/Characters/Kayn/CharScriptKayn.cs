using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AI脚本: Kayn - 暗影杀手/掠食者
    /// 被动技能: 暗影杀手 (Shadow Assassin) / 掠食者 (Rhaast)
    /// </summary>
    public class CharScriptKayn : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Kayn有三个形态: 普通形态, 暗影杀手(蓝), 掠食者(红)
            // 蓝: 短时间爆发伤害
            // 红: 持续战斗, 治疗
        }
    }
}