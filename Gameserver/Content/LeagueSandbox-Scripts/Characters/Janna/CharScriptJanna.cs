using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Janna - The Storm's Spirit
    /// </summary>
    public class CharScriptJanna : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Janna被动技能: Tailwind - 友军移动速度提升
        }
    }
}