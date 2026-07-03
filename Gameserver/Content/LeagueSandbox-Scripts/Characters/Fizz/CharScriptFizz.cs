using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Fizz - The Tidal Trickster
    /// </summary>
    public class CharScriptFizz : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Fizz被动技能: Nimble Fighter - 获得无视碰撞和减伤
        }
    }
}