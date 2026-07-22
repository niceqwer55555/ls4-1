using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Vayne - The Night Hunter
    /// </summary>
    public class CharScriptVayne : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Vayne被动技能: Night Hunter - 获得移速加成
        }
    }
}