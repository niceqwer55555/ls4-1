using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;

namespace Buffs
{
    /// <summary>Syndra Q - 球体计数buff (用于R伤害计算)</summary>
    public class SyndraQCount : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        { BuffType = GameServerCore.Enums.BuffType.COUNTER, BuffAddType = GameServerCore.Enums.BuffAddType.STACKS_AND_RENEWS, MaxStacks = 4 };
        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell) { }
        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell) { }
        public void OnUpdate(float diff) { }
    }
}





