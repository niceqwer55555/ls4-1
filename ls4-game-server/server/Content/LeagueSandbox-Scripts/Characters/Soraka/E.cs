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
    /// Soraka E - Infuse
    /// 对友军使用：恢复法力值
    /// 对敌人使用：造成魔法伤害并沉默
    /// </summary>
    public class SorakaE : ISpellScript
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

            // 法力回复: 40/80/120/160/200
            float[] manaRestore = { 40f, 80f, 120f, 160f, 200f };
            float mana = manaRestore[spell.CastInfo.SpellLevel - 1];

            // 沉默持续时间: 1.5/1.75/2/2.25/2.5秒
            float[] silenceDuration = { 1.5f, 1.75f, 2f, 2.25f, 2.5f };
            float silence = silenceDuration[spell.CastInfo.SpellLevel - 1];

            if (target.Team == owner.Team)
            {
                // 友军 - 恢复法力
                target.Stats.CurrentMana = Math.Min(target.Stats.CurrentMana + mana, target.Stats.ManaPoints.Total);
            }
            else
            {
                // 敌人 - 造成魔法伤害并沉默
                // 伤害: 50/100/150/200/250 (+0.6 AP)
                float[] baseDamage = { 50f, 100f, 150f, 200f, 250f };
                float ap = owner.Stats.AbilityPower.Total * 0.6f;
                float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                AddBuff("Silence", silence, 1, spell, target, owner);
            }
        }
    }
}
