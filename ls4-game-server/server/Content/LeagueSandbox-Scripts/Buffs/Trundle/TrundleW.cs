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
    public class TrundleW : IBuffGameScript
    {
        
        
        
        

                public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // Frozen Domain (W): 增加移动速度和攻击速度
            StatsModifier.MoveSpeed.PercentBonus = 0.3f;
            StatsModifier.AttackSpeed.PercentBonus = 0.3f;
            unit.AddStatModifier(StatsModifier);

            AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Trundle_W_Icon.troy", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.RemoveStatModifier(StatsModifier);
        }

        
    }
}




