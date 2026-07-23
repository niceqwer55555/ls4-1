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
    public class RakanE : IBuffGameScript
    {
        
        
        
        

                public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.STACKS_AND_CONTINUE
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // Battle Dance (E): 位移到友军身边并为其提供护盾
            unit.Stats.MagicResist.FlatBonus = 40f;

            AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Rakan_E_Icon.troy", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.Stats.MagicResist.FlatBonus = 0f;
        }

        
    }
}




