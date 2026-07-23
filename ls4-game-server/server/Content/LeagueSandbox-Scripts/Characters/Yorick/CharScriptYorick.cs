using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AI脚本: Yorick - 痛苦牧魂人
    /// 被动技能: 牧魂 (Soul Harvest)
    /// </summary>
    public class CharScriptYorick : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Yorick被动: 召唤迷雾室女和僵尸
        }
    }
}