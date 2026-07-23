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
    /// Soraka Q - Starcall
    /// 召唤流星坠落，对范围内敌人造成魔法伤害并减速
    /// 如果命中敌方英雄，索拉卡恢复生命值
    /// </summary>
    public class SorakaQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 魔法伤害: 60/85/110/135/160 (+0.4 AP)
            float[] baseDamage = { 60f, 85f, 110f, 135f, 160f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对范围内的敌人造成伤害
            foreach (var unit in GetUnitsInRange(targetPos, 530f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 减速30%持续2秒
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }

            // 命中英雄后索拉卡恢复生命值
            // 4秒内恢复8/11/14/17/20 (+5% AP)
            AddBuff("SorakaHeal", 4f, 1, spell, owner, owner);
            AddParticle(owner, owner, "SorakaQ_cas", targetPos);
        }
    }
}
