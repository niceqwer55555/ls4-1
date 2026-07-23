using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Nami - The Tidecaller
    /// </summary>
    public class CharScriptNami : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Nami被动技能: Surging Tides - 技能给友军加速
        }
    }
}