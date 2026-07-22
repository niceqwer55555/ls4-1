using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Leona - The Radiant Dawn
    /// </summary>
    public class CharScriptLeona : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Leona被动技能: Sunlight - 技能标记敌人，友军攻击触发伤害
        }
    }
}