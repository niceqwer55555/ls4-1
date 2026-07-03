using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Malphite -Shard of the Monolith
    /// </summary>
    public class CharScriptMalphite : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Malphite被动技能: Granite Shield - 获得护盾
        }
    }
}