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
    /// Zilean W - Rewind
    /// 减少其他技能冷却时间
    /// </summary>
    public class ZileanW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 减少其他技能冷却: 10/15/20/25/30%
            float[] cooldownReduction = { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f };
            float reduction = cooldownReduction[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 3秒
            AddBuff("ZileanW", 3f, 1, spell, owner, owner);
        }
    }
}
