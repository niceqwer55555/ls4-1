using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Aatrox - The Darkin Blade
    /// </summary>
    public class CharScriptAatrox : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Aatrox被动技能: Blood Well - 死亡后复活
        }
    }
}