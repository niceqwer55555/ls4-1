using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Varus - The Arrow of Retribution
    /// </summary>
    public class CharScriptVarus : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Varus被动技能: Life Steal - 获得生命偷取
        }
    }
}