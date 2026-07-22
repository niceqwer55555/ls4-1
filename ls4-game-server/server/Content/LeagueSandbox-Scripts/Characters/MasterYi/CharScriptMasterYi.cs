using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// MasterYi - The Wuju Bladesman
    /// </summary>
    public class CharScriptMasterYi : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // MasterYi被动技能: Double Strike - 连续攻击两次
        }
    }
}