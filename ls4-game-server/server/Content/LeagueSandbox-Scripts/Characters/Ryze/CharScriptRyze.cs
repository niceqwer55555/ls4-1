using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Ryze - The Rune Mage
    /// </summary>
    public class CharScriptRyze : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Ryze被动技能: Arcane Mastery - 技能减少其他技能冷却
        }
    }
}