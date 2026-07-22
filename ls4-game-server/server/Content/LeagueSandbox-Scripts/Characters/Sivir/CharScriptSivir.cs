using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Sivir - The Battle Mistress
    /// </summary>
    public class CharScriptSivir : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Sivir被动技能: Fleet of Foot - 获得移速加成
        }
    }
}