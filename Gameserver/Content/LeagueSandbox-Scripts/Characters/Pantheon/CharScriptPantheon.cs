using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Pantheon - The Artisan of War
    /// </summary>
    public class CharScriptPantheon : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Pantheon被动技能: Mortal Will - 技能叠层提升伤害
        }
    }
}