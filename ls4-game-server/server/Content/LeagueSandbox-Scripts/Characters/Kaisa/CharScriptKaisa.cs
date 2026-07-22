using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Kaisa - Daughter of the Void
    /// </summary>
    public class CharScriptKaisa : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Kaisa被动技能: Supercharge - 技能叠层提升伤害
        }
    }
}