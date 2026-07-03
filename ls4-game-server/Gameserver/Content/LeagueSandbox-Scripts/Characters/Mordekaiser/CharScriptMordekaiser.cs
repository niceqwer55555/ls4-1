using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Mordekaiser - The Iron Revenant
    /// </summary>
    public class CharScriptMordekaiser : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Mordekaiser被动技能: Iron Man - 护盾吸收伤害
        }
    }
}