using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// XinZhao - The Resistance
    /// </summary>
    public class CharScriptXinZhao : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // XinZhao被动技能: Audacious Charge - 技能减少冷却
        }
    }
}