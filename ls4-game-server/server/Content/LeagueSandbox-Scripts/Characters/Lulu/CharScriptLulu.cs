using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// Lulu - The Fae Sorceress
    /// </summary>
    public class CharScriptLulu : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // Lulu被动技能: Pix's Companion - 伴随的小精灵提供额外伤害
        }
    }
}