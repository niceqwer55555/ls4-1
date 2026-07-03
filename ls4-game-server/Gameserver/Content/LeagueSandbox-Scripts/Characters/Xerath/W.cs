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
    /// Xerath W - Eye of Destruction
    /// 释放毁灭之眼伤害并减速敌人
    /// </summary>
    public class XerathW : ISpellScript
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

            // 魔法伤害: 60/90/120/150/180 (+0.6 AP)
            float[] baseDamage = { 60f, 90f, 120f, 150f, 180f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 减速: 25/30/35/40/45%
            float[] slowAmount = { 0.25f, 0.30f, 0.35f, 0.40f, 0.45f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];

            foreach (var unit in GetUnitsInRange(targetPos, 250f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "XerathW_cas", targetPos);
        }
    }
}
