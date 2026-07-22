using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Singed - The Mad Chemist
    /// </summary>
    public class CharScriptSinged : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Singed被动技能: Llative - 获得额外移速
        }
    }
}