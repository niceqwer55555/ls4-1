using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AI脚本: Rell - 钢铁少女
    /// 被动技能: 磁力冲击 (Magnetize)
    /// </summary>
    public class CharScriptRell : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Rell被动: 攻击敌人时获得护甲和魔抗
        }
    }
}