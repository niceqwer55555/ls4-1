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
    public class JayceR : IBuffGameScript
    {
        
        
        
        public bool IsHidden => true;

                public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // Transform: Mercury Cannon (R): 切换到锤形态获得护甲和魔抗
            unit.Stats.Armor.FlatBonus = 20f;
            unit.Stats.MagicResist.FlatBonus = 20f;
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.Stats.Armor.FlatBonus = 0f;
            unit.Stats.MagicResist.FlatBonus = 0f;
        }

        
    }
}




