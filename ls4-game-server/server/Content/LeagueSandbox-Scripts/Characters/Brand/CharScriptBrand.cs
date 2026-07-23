using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Brand - The Firemage
    /// </summary>
    public class CharScriptBrand : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Brand被动技能: Pyroclasm - 技能在敌人间弹射
        }
    }
}