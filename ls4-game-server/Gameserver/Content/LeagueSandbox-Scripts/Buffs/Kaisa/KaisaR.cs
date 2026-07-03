using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.GameObjects;

namespace Buffs
{
    public class KaisaR : IBuffGameScript
    {
        
        
        
        

                public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // Killer Instinct (R): 位移到目标身后并获得护盾
            var owner = ownerSpell.CastInfo.Owner;
            float shieldAmount = 100f + owner.Stats.AbilityPower.Total * 0.8f;
            
            unit.SetStatusEffect(StatusFlags.Invulnerable, true);

            AddParticleTarget(owner, unit, "Kaisa_R_Icon.troy", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.SetStatusEffect(StatusFlags.Invulnerable, false);
        }

        
    }
}




