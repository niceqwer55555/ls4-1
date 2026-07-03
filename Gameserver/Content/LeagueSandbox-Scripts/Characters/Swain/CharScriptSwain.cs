using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Swain - The Grand General
    /// </summary>
    public class CharScriptSwain : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Swain被动技能: Vision of the Empire - 技能揭露敌人
        }
    }
}