using System;
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
    /// Soraka W - Astral Blessing
    /// 治疗一名友军并给予3秒护甲加成
    /// </summary>
    public class SorakaW : ISpellScript
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

            // 治疗: 70/140/210/280/350 (+0.45 AP)
            float[] healAmount = { 70f, 140f, 210f, 280f, 350f };
            float ap = owner.Stats.AbilityPower.Total * 0.45f;
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 治疗目标
            target.Stats.CurrentHealth = Math.Min(target.Stats.CurrentHealth + heal, target.Stats.HealthPoints.Total);

            // 添加护甲加成buff 3秒
            // 护甲加成: 25/45/65/85/105
            float[] armorBonus = { 25f, 45f, 65f, 85f, 105f };
            float armor = armorBonus[spell.CastInfo.SpellLevel - 1];
            AddBuff("ArmorBuff", 3f, 1, spell, target, owner);
        }
    }
}
