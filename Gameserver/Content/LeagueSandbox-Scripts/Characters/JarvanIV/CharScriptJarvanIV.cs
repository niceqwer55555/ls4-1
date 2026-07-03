using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// JarvanIV - The Exemplar of Demacia
    /// </summary>
    public class CharScriptJarvanIV : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // JarvanIV被动技能: Martial Cadence - 攻击造成额外伤害
        }
    }
}