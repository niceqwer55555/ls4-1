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
    /// Sona Q - Hymn of Valor
    /// 释放音符对范围内两个敌人造成魔法伤害
    /// </summary>
    public class SonaQ : ISpellScript
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

            // 伤害: 50/85/120/155/190 (+40% AP)
            float[] baseDamage = { 50f, 85f, 120f, 155f, 190f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 对范围内两个敌人造成伤害
            int hitCount = 0;
            foreach (var unit in GetUnitsInRange(owner.Position, 850f, true))
            {
                if (unit.Team != owner.Team && hitCount < 2)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    hitCount++;
                }
            }

            // 获得30法力（第一次命中友军时）
            AddBuff("SonaMana", 5f, 1, spell, owner, owner);
            AddParticle(owner, owner, "SonaQ_cas", owner.Position);
        }
    }
}
