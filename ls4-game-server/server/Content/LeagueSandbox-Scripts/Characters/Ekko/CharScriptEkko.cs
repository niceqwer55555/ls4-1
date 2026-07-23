using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Ekko - The Boy Who Shattered Time
    /// </summary>
    public class CharScriptEkko : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Ekko被动技能: Z-Drive Resonance - 技能命中叠层，提升伤害
        }
    }
}