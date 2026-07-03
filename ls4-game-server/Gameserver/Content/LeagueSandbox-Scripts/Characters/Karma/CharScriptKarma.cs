using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Karma - The Enlightened One
    /// </summary>
    public class CharScriptKarma : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Karma被动技能: Gathering Fire - 技能减少大招冷却
        }
    }
}