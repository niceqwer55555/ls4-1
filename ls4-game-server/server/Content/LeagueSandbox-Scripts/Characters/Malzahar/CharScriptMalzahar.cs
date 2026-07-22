using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Malzahar - The Void's Prophet
    /// </summary>
    public class CharScriptMalzahar : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Malzahar被动技能: Void Shift - 获得移动和法术伤害加成
        }
    }
}