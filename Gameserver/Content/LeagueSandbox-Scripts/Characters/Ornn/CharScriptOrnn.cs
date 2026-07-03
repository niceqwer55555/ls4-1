using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Ornn - The Fire below the Mountain
    /// </summary>
    public class CharScriptOrnn : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Ornn被动技能: Living Forge - 可以制作装备
        }
    }
}