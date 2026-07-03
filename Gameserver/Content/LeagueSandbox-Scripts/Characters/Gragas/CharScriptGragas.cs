using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Gragas - The Rabble Rouser
    /// </summary>
    public class CharScriptGragas : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Gragas被动技能: Happy Hour - 使用技能后回复生命值
        }
    }
}