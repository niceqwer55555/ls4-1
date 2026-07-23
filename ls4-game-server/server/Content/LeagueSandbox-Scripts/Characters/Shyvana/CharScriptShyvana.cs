using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Shyvana - The Half-Dragon
    /// </summary>
    public class CharScriptShyvana : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Shyvana被动技能: Dragonborn - 战斗获得怒气，变身后获得属性加成
        }
    }
}