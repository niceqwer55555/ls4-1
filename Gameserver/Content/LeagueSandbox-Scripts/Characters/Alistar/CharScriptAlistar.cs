using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Alistar - The Minotaur
    /// </summary>
    public class CharScriptAlistar : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Alistar被动技能: Sadism - 获得治疗和伤害加成
        }
    }
}