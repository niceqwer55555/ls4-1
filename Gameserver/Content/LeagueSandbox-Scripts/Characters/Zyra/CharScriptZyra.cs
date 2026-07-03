using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Zyra - Rose of the Ruination
    /// </summary>
    public class CharScriptZyra : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Zyra被动技能: Garden of Thorns - 植物获得额外伤害
        }
    }
}