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
    /// Rammus E - Puncturing Taunt
    /// 嘲讽敌人并减少其护甲
    /// </summary>
    public class RammusE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;

            // 护甲减少: 10/15/20/25/30
            float[] armorReduction = { 10f, 15f, 20f, 25f, 30f };
            float reduction = armorReduction[spell.CastInfo.SpellLevel - 1];

            // 嘲讽持续时间: 1/1.5/2/2.5/3秒
            float[] duration = { 1f, 1.5f, 2f, 2.5f, 3f };
            float dur = duration[spell.CastInfo.SpellLevel - 1];

            AddBuff("Taunt", dur, 1, spell, target, owner);
        }
    }
}
