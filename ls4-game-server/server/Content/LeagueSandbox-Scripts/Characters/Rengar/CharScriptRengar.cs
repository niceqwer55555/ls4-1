using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Rengar - The Pridestalker
    /// </summary>
    public class CharScriptRengar : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Rengar被动技能: Unseen Predator - 草丛隐身
        }
    }
}