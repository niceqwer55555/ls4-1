using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;

namespace Spells
{
    /// <summary>
    /// Vladimir W - Sanguine Pool
    /// 变成血池无法被选中
    /// </summary>
    public class VladimirW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 持续时间: 2/2.5/3/3.5/4秒
            float[] duration = { 2f, 2.5f, 3f, 3.5f, 4f };
            float dur = duration[spell.CastInfo.SpellLevel - 1];

            AddBuff("VladimirW", dur, 1, spell, owner, owner);
        }
    }
}
