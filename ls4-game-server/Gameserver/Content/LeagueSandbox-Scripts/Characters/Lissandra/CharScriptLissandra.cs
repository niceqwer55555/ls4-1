using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Lissandra - The Ice Witch
    /// </summary>
    public class CharScriptLissandra : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Lissandra被动技能: Iceborn - 技能减速敌人
        }
    }
}