using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace Buffs
{
    /// <summary>
    /// Teemo E - Toxic Shot poison debuff.
    /// Deals poison damage over time.
    /// </summary>
    public class ToxicShot : IBuffGameScript
    {
        float damageTickTimer = 0f;
        AttackableUnit toxicTarget;
        ObjAIBase toxicOwner;
        Spell toxicSpell;

        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_DEBUFF,
            BuffAddType = BuffAddType.STACKS_AND_CONTINUE
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            toxicTarget = unit;
            toxicOwner = ownerSpell.CastInfo.Owner as ObjAIBase;
            toxicSpell = ownerSpell;
            damageTickTimer = 0f;
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (toxicOwner == null || toxicTarget == null || toxicTarget.IsDead)
            {
                return;
            }

            damageTickTimer += diff / 1000f;
            if (damageTickTimer >= 1f)
            {
                damageTickTimer = 0f;

                // Poison DoT per second: 6/12/18/24/30 (+10% AP)
                var spellLevel = toxicSpell.CastInfo.SpellLevel;
                if (spellLevel <= 0) return;

                float[] dotDamage = { 6f, 12f, 18f, 24f, 30f };
                float ap = toxicOwner.Stats.AbilityPower.Total * 0.1f;
                float damage = dotDamage[spellLevel - 1] + ap;

                toxicTarget.TakeDamage(toxicOwner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLPERSIST, false);
            }
        }
    }
}
