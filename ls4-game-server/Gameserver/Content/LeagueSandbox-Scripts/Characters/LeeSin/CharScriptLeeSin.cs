using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace CharScripts
{
    /// <summary>
    /// LeeSin - The Blind Monk
    /// </summary>
    public class CharScriptLeeSin : ICharScript
    {
        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            // LeeSin被动技能: Flurry - 使用技能后获得攻速加成
        }
    }
}
