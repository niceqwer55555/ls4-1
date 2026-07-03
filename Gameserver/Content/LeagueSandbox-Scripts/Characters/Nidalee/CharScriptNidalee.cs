using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Nidalee - The Bestial Huntress
    /// </summary>
    public class CharScriptNidalee : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Nidalee被动技能: Prowl - 草丛移动获得加速
        }
    }
}