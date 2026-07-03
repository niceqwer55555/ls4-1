using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Zac - The Secret Weapon
    /// </summary>
    public class CharScriptZac : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Zac被动技能: Cell Division - 死亡后复活
        }
    }
}