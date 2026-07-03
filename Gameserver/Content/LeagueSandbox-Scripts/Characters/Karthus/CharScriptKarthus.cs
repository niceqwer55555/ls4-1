using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Karthus - The Deathsinger
    /// </summary>
    public class CharScriptKarthus : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Karthus被动技能: Death Defied - 死亡后继续放技能
        }
    }
}