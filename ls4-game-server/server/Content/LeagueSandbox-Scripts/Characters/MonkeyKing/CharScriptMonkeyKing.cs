using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// MonkeyKing - The Monkey King
    /// </summary>
    public class CharScriptMonkeyKing : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // MonkeyKing被动技能: Stone Skin - 获得护甲和魔抗
        }
    }
}