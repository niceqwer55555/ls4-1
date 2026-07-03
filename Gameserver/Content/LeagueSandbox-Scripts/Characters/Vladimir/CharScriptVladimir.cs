using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Vladimir - The Crimson Reaper
    /// </summary>
    public class CharScriptVladimir : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Vladimir被动技能: Crimson Pact - 获得额外生命值和法术强度转换
        }
    }
}