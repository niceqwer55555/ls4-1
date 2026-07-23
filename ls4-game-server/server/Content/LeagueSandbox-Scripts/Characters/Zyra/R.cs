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
    /// Zyra R - Strangle
    /// 召唤大型植物攻击敌人
    /// </summary>
    public class ZyraR : ISpellScript
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

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 每秒魔法伤害: 80/120/160 (+0.7 AP)
            float[] dpsDamage = { 80f, 120f, 160f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float dps = dpsDamage[spell.CastInfo.SpellLevel - 1] + ap;

            foreach (var unit in GetUnitsInRange(targetPos, 500f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, dps, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 击飞
                    AddBuff("Knockup", 0.5f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "ZyraR_cas", targetPos);
        }
    }
}
