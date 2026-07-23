using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Kindred - The Eternal Hunters
    /// </summary>
    public class CharScriptKindred : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Kindred被动技能: Mark of the Kindred - 标记敌人击杀获得奖励
        }
    }
}