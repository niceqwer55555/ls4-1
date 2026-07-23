using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Fiddlesticks - The Harbinger of Doom
    /// </summary>
    public class CharScriptFiddlesticks : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Fiddlesticks被动技能: Crow Storm - 大招恐惧敌人
        }
    }
}