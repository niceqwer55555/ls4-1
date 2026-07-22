using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// DrMundo - The Madman of Zaun
    /// </summary>
    public class CharScriptDrMundo : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // DrMundo被动技能: Adrenaline Rush - 移动和攻击回复生命值
        }
    }
}