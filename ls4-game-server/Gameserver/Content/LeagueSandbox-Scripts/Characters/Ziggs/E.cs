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
    /// Ziggs E - Minefield
    /// 布置地雷区伤害并减速敌人
    /// </summary>
    public class ZiggsE : ISpellScript
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

            // 每颗魔法伤害: 40/65/90/115/140 (+0.3 AP)
            float[] baseDamage = { 40f, 65f, 90f, 115f, 140f };
            float ap = owner.Stats.AbilityPower.Total * 0.3f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 减速: 30%
            float slow = 0.30f;

            foreach (var unit in GetUnitsInRange(targetPos, 300f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "ZiggsE_cas", targetPos);
        }
    }
}
