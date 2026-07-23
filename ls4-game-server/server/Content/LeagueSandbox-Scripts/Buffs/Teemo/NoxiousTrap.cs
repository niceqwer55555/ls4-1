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
    /// Noxious Trap debuff - applied when an enemy triggers Teemo's mushroom.
    /// Deals damage over 4 seconds and slows.
    /// </summary>
    public class NoxiousTrap : IBuffGameScript
    {
        float damageTickTimer = 0f;
        AttackableUnit debuffTarget;
        ObjAIBase debuffOwner;
        Spell debuffSpell;

        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_DEBUFF,
            BuffAddType = BuffAddType.STACKS_AND_CONTINUE
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            debuffTarget = unit;
            debuffOwner = ownerSpell.CastInfo.Owner as ObjAIBase;
            debuffSpell = ownerSpell;
            damageTickTimer = 0f;
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (debuffOwner == null || debuffTarget == null || debuffTarget.IsDead)
            {
                return;
            }

            damageTickTimer += diff / 1000f;
            if (damageTickTimer >= 1f)
            {
                damageTickTimer = 0f;

                // Damage per tick: 50/81.25/112.5 (+12.5% AP) per second for 4 seconds
                // Total: 200/325/450 (+50% AP) over 4 seconds
                var spellLevel = debuffSpell.CastInfo.SpellLevel;
                if (spellLevel <= 0) return;

                float[] totalDamage = { 200f, 325f, 450f };
                float ap = debuffOwner.Stats.AbilityPower.Total * 0.5f;
                float tickDamage = (totalDamage[spellLevel - 1] + ap) / 4f;

                debuffTarget.TakeDamage(debuffOwner, tickDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLPERSIST, false);
            }
        }
    }
}
