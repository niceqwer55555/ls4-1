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
    /// Braum E - Unbreakable
    /// 举起盾牌抵挡伤害
    /// </summary>
    public class BraumE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 伤害格挡: 30/50/70/90/110
            float[] damageBlock = { 30f, 50f, 70f, 90f, 110f };
            float block = damageBlock[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 3/3.5/4/4.5/5秒
            float[] duration = { 3f, 3.5f, 4f, 4.5f, 5f };
            float dur = duration[spell.CastInfo.SpellLevel - 1];

            AddBuff("BraumE", dur, 1, spell, owner, owner);
        }
    }
}
