using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Xerath - The Magus Ascendant
    /// </summary>
    public class CharScriptXerath : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Xerath被动技能: Arcane Mastery - 获得额外法术强度
        }
    }
}