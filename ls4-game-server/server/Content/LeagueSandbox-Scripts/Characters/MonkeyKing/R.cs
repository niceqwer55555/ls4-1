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
    /// MonkeyKing R - Cyclone
    /// 旋转并伤害周围敌人
    /// </summary>
    public class MonkeyKingR : ISpellScript
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

            // 每秒物理伤害: 100/150/200 (+1.1 bonus AD)
            float[] dpsDamage = { 100f, 150f, 200f };
            float bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            float dps = dpsDamage[spell.CastInfo.SpellLevel - 1] + bonusAd * 1.1f;

            foreach (var unit in GetUnitsInRange(owner.Position, 350f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, dps, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 持续时间: 8秒
            AddBuff("MonkeyKingR", 8f, 1, spell, owner, owner);

            AddParticle(owner, owner, "MonkeyKingR_cas", owner.Position);
        }
    }
}
