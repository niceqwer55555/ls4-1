using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Jayce - The Defender of Tomorrow
    /// </summary>
    public class CharScriptJayce : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Jayce被动技能: Mercury Hammer/Hammer Stance - 变形获得额外属性
        }
    }
}