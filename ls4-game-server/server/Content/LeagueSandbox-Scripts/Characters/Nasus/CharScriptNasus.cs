using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Nasus - The Curator of the Sands
    /// </summary>
    public class CharScriptNasus : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Nasus被动技能: Soul Eater - 击杀回复生命值
        }
    }
}