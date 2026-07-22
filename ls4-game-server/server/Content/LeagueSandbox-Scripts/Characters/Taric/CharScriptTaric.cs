using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Taric - The Shield of Valoran
    /// </summary>
    public class CharScriptTaric : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Taric被动技能: Bravado - 技能和攻击获得增益
        }
    }
}