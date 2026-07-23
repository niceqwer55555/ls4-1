using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Nocturne - The Eternal Nightmare
    /// </summary>
    public class CharScriptNocturne : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Nocturne被动技能: Unspeakable Horror - 技能造成恐惧
        }
    }
}