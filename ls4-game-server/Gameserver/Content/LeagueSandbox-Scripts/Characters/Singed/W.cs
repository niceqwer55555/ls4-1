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
    /// Singed W - Mega Adhesive
    /// 投掷巨型粘液区域，减速并定身敌人
    /// </summary>
    public class SingedW : ISpellScript
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

            // 魔法伤害: 60/100/140/180/220 (+0.6 AP)
            float[] baseDamage = { 60f, 100f, 140f, 180f, 220f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 减速: 35/45/55/65/75%
            float[] slowAmount = { 0.35f, 0.45f, 0.55f, 0.65f, 0.75f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];

            // 定身: 1/1.25/1.5/1.75/2秒
            float[] duration = { 1f, 1.25f, 1.5f, 1.75f, 2f };

            foreach (var unit in GetUnitsInRange(targetPos, 350f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Slow", duration[spell.CastInfo.SpellLevel - 1], 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "SingedW_cas", targetPos);
        }
    }
}
