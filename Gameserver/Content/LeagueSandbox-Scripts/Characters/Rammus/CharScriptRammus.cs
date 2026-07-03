using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Rammus - The Armordillo
    /// </summary>
    public class CharScriptRammus : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Rammus被动技能: Spikes - 受到攻击时反弹伤害
        }
    }
}