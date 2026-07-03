using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// MissFortune - The Bounty Hunter
    /// </summary>
    public class CharScriptMissFortune : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // MissFortune被动技能: Strut - 站立不动时获得移速加成
        }
    }
}