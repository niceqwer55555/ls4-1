using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.API;

namespace Buffs
{
    /// <summary>
    /// MasterYi E Active Buff - Wuju Style activated.
    /// Attacks deal bonus true damage. Passive AD bonus becomes 10/12.5/15/17.5/20%.
    /// When this buff expires, passive buff is re-applied.
    /// </summary>
    internal class WujuStyleSuperChargedVisual : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        private ObjAIBase Owner;
        private Spell daSpell;
        private int spellLevel;

        // Active AD bonus: 10/12.5/15/17.5/20% (replaces passive 10%)
        float[] activeAdPercent =
        {
            0.10f,
            0.125f,
            0.15f,
            0.175f,
            0.20f
        };

        // Base true damage on hit: 10/15/20/25/30
        float[] baseTrueDamage =
        {
            10f,
            15f,
            20f,
            25f,
            30f
        };

        // AD ratio for true damage: 10/12.5/15/17.5/20%
        float[] adRatioPercent =
        {
            0.10f,
            0.125f,
            0.15f,
            0.175f,
            0.20f
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            Owner = ownerSpell.CastInfo.Owner;
            daSpell = ownerSpell;
            spellLevel = ownerSpell.CastInfo.SpellLevel - 1;

            // Apply active AD bonus (10/12.5/15/17.5/20%)
            StatsModifier.AttackDamage.PercentBonus = activeAdPercent[spellLevel];
            unit.AddStatModifier(StatsModifier);

            // Seal E spell slot while active
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, true);

            // Listen for auto-attack launches to apply true damage
            ApiEventManager.OnLaunchAttack.AddListener(this, Owner, OnLaunchAttack, false);
        }

        /// <summary>
        /// Apply bonus true damage on auto-attack hit.
        /// </summary>
        public void OnLaunchAttack(Spell spell)
        {
            var owner = daSpell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;

            // True damage: base + (AD ratio * total AD)
            var damage = baseTrueDamage[spellLevel] + owner.Stats.AttackDamage.Total * adRatioPercent[spellLevel];
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // Remove listeners
            ApiEventManager.OnLaunchAttack.RemoveListener(this);
            ApiEventManager.OnLaunchAttack.RemoveListener(this, Owner);

            // Remove stat modifier
            unit.RemoveStatModifier(StatsModifier);

            // Unlock E spell slot
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, false);

            // Re-apply passive buff
            AddBuff("WujuStylePassive", 1f, 1, daSpell, Owner, Owner, true);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
