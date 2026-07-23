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
    /// Leona R - Solar Flare
    /// 召唤太阳光芒，对范围内敌人造成魔法伤害
    /// 眩晕并减速他们
    /// </summary>
    public class SolarFlare : ISpellScript
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

            // 伤害公式: 150/250/350 + 0.8 AP
            float[] baseDamage = { 150f, 250f, 350f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对范围内敌人造成伤害
            foreach (var unit in GetUnitsInRange(targetPos, 400f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 眩晕1.5秒
                    AddBuff("Stun", 1.5f, 1, spell, unit, owner);
                    // 减速80%持续3秒
                    AddBuff("Slow", 3f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "SolarFlare_cas", targetPos);
        }
    }
}
