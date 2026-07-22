using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Poppy - The Iron Ambassador
    /// </summary>
    public class CharScriptPoppy : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Poppy被动技能: Iron Ambassador - 获得护盾
        }
    }
}