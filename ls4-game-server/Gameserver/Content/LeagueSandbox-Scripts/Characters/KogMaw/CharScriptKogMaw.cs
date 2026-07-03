using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// KogMaw - The Mouth of the Abyss
    /// </summary>
    public class CharScriptKogMaw : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // KogMaw被动技能: Organic Decay - 死亡后造成范围伤害
        }
    }
}