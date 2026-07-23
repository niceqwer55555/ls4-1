using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Olaf - The Berserker
    /// </summary>
    public class CharScriptOlaf : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Olaf被动技能: Berserker Rage - 攻击速度提升
        }
    }
}