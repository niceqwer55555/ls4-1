using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Miss Fortune E - Make It Rain
    /// 在指定区域降下弹雨，造成伤害并减速敌人
    /// </summary>
    public class MissFortuneScattered : ISpellScript
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

            var castPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 伤害公式: 90/145/200/255/310 + 0.8 AP
            float[] baseDamage = { 90f, 145f, 200f, 255f, 310f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 对范围内敌人造成伤害
            foreach (var unit in GetUnitsInRange(castPos, 400f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL,
                        DamageSource.DAMAGE_SOURCE_SPELLAOE, false);

                    // 减速效果
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }

            // 粒子效果
            AddParticle(owner, owner, "MissFortune_E_cas", castPos, lifetime: 0.5f);
        }

        public void OnSpellPostCast(Spell spell) { }
        public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}