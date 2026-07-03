using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AI脚本: Sylas - 窃法魔医
    /// 被动技能: 盗取 (Plunder)
    /// </summary>
    public class CharScriptSylas : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Sylas被动: 技能命中时回复法力值
        }
    }
}