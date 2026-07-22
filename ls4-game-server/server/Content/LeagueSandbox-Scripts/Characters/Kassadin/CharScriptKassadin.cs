using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Kassadin - The Void Walker
    /// </summary>
    public class CharScriptKassadin : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Kassadin被动技能: Void Stone - 减少15%魔法伤害并忽视单位碰撞
        }
    }
}
