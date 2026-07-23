using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Diana R - Moonfall
    /// 吸引并显形身边的所有敌人，使其减速
    /// 如果拉到至少一个敌方英雄，月光会再次降临，造成范围伤害
    /// </summary>
    public class Moonfall : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 减速效果: 35/40/45/50/55% 持续2秒
            float[] slowAmount = { 0.35f, 0.40f, 0.45f, 0.50f, 0.55f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];

            int championsPulled = 0;

            // 吸引并减速所有范围内的敌人
            foreach (var unit in GetUnitsInRange(owner.Position, 250f, true))
            {
                if (unit.Team != owner.Team)
                {
                    // 减速
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                    championsPulled++;
                }
            }

            // 如果拉到至少一个英雄，月光降临造成范围伤害
            if (championsPulled > 0)
            {
                // 伤害公式: 200 + 35 * (championsPulled - 1) + 0.8 AP
                float[] baseDamage = { 200f, 250f, 300f };
                float ap = owner.Stats.AbilityPower.Total * 0.8f;
                float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

                foreach (var unit in GetUnitsInRange(owner.Position, 400f, true))
                {
                    if (unit.Team != owner.Team)
                    {
                        unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    }
                }
            }

            AddParticle(owner, owner, "Moonfall_cas", owner.Position);
        }
    }
}
