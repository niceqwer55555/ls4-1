using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Bard - The Wandering Caretaker
    /// </summary>
    public class CharScriptBard : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Bard被动技能: Tempered Fate - 圣所对敌人减速，对友军加速
        }
    }
}