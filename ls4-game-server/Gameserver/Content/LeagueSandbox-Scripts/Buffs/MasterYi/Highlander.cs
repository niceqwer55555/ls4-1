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
    internal class Highlander : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER
        };

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        Particle highlander;
        Champion owner;
        Buff thisBuff;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as Champion;
            thisBuff = buff;
            highlander = AddParticleTarget(owner, unit, "Highlander_buf", unit);

            StatsModifier.MoveSpeed.PercentBonus = StatsModifier.MoveSpeed.PercentBonus + (15f + ownerSpell.CastInfo.SpellLevel * 10) / 100f;
            StatsModifier.AttackSpeed.PercentBonus = StatsModifier.AttackSpeed.PercentBonus + (5f + ownerSpell.CastInfo.SpellLevel * 25) / 100f;
            StatsModifier.SlowResistPercent = 1.0f;
            unit.AddStatModifier(StatsModifier);

            ApiEventManager.OnKill.AddListener(this, owner, OnKill, false);
        }

        public void OnKill(DeathData data)
        {
            if (data.Killer == owner)
            {
                owner.GetSpell("AlphaStrike").SetCooldown(0);
                owner.GetSpell("Meditate").SetCooldown(0);
                owner.GetSpell("WujuStyle").SetCooldown(0);
                RefreshBuffDuration();
            }
        }

        public void RefreshBuffDuration()
        {
            if (thisBuff != null)
            {
                thisBuff.ResetTimeElapsed();
            }
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            ApiEventManager.OnKill.RemoveListener(this);
            RemoveParticle(highlander);
        }
    }
}
