using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Morgana - The Fallen
    /// </summary>
    public class CharScriptMorgana : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Morgana被动技能: Soul Siphon - 技能造成伤害时回复
        }
    }
}