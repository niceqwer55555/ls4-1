using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Kennen - The Heart of the Tempest
    /// </summary>
    public class CharScriptKennen : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Kennen被动技能: Electrical Storm - 技能命中标记敌人
        }
    }
}