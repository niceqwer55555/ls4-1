using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Lux - The Lady of Luminosity
    /// </summary>
    public class CharScriptLux : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Lux被动技能: Illumination - 技能标记敌人，攻击触发额外伤害
        }
    }
}