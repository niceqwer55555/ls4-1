using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Pyke - The Bloodharbor Ripper
    /// </summary>
    public class CharScriptPyke : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Pyke被动技能: Gift of the Drowned - 获得额外金币
        }
    }
}