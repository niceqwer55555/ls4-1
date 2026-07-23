using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerLib.GameObjects.AttackableUnits;

namespace Buffs
{
    /// <summary>
    /// MasterYi R - Highlander buff
    /// Move speed: 25/35/45%, Attack speed: 30/55/80%, Slow resist: 100%.
    /// Kills reset Q/W/E cooldowns and extend buff duration.
    /// </summary>
    internal class Highlander : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        Particle highlander;
        Champion owner;
        Buff thisBuff;

        // Move speed bonus: 25/35/45%
        float[] moveSpeedPercent =
        {
            0.25f,
            0.35f,
            0.45f
        };

        // Attack speed bonus: 30/55/80%
        float[] attackSpeedPercent =
        {
            0.30f,
            0.55f,
            0.80f
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as Champion;
            thisBuff = buff;
            highlander = AddParticleTarget(owner, unit, "Highlander_buf", unit);

            var level = ownerSpell.CastInfo.SpellLevel - 1;

            // Apply move speed, attack speed, and slow resist
            StatsModifier.MoveSpeed.PercentBonus = moveSpeedPercent[level];
            StatsModifier.AttackSpeed.PercentBonus = attackSpeedPercent[level];
            StatsModifier.SlowResistPercent = 1.0f;
            unit.AddStatModifier(StatsModifier);

            // Listen for kills to reset cooldowns and extend buff
            ApiEventManager.OnKill.AddListener(this, owner, OnKill, false);
        }

        /// <summary>
        /// On kill: reset Q/W/E cooldowns and extend buff duration.
        /// </summary>
        public void OnKill(DeathData data)
        {
            if (data.Killer == owner)
            {
                // Reset basic ability cooldowns
                owner.GetSpell("AlphaStrike").SetCooldown(0f);
                owner.GetSpell("Meditate").SetCooldown(0f);
                owner.GetSpell("WujuStyle").SetCooldown(0f);

                // Extend buff duration by 4 seconds
                ExtendBuffDuration();
            }
        }

        /// <summary>
        /// Extend buff duration by 4 seconds (reset elapsed time).
        /// </summary>
        public void ExtendBuffDuration()
        {
            if (thisBuff != null)
            {
                thisBuff.ResetTimeElapsed();
            }
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            ApiEventManager.OnKill.RemoveListener(this);
            unit.RemoveStatModifier(StatsModifier);
            RemoveParticle(highlander);
        }
    }
}
