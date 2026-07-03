using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// RekSai - The Void Burrower
    /// </summary>
    public class CharScriptRekSai : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // RekSai被动技能: Fury of the Xer-Sai - 攻击获得怒气
        }
    }
}