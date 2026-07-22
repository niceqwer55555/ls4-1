using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Seraphine - The Starry-Eyed Singer
    /// </summary>
    public class CharScriptSeraphine : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Seraphine被动技能: Stage Presence - 技能可以给友军演奏
        }
    }
}