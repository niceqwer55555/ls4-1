using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// AI脚本: Naafiri - 噬星猎鼠
    /// 被动技能: 暗影之王 (Darkin Blade)
    /// </summary>
    public class CharScriptNaafiri : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Naafiri被动: 暗影刀伤害
        }
    }
}