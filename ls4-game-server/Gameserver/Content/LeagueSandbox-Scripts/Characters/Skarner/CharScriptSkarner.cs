using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Skarner - The Crystal Vanguard
    /// </summary>
    public class CharScriptSkarner : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Skarner被动技能: Crystal Sting - 技能标记敌人
        }
    }
}