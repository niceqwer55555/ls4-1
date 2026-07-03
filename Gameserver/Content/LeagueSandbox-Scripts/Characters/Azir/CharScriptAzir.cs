using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Azir - The Emperor of the Shurima
    /// </summary>
    public class CharScriptAzir : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Azir被动技能: Shurima's Legacy - 友军士兵变成哨兵
        }
    }
}