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
using GameServerCore.Enums;

namespace Buffs
{
    /// <summary>
    /// Anivia W - 冰墙生命周期 (5秒后消失)
    /// </summary>
    public class AniviaWallLife : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = GameServerCore.Enums.BuffType.INTERNAL,
            BuffAddType = GameServerCore.Enums.BuffAddType.REPLACE_EXISTING,
            MaxStacks = 1
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // 冰墙�?秒后由buff持续时间自动触发OnDeactivate
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            // 移除冰墙单位
            unit.TakeDamage(unit, 99999f, DamageType.DAMAGE_TYPE_TRUE,
                DamageSource.DAMAGE_SOURCE_RAW, false);
        }

        public void OnUpdate(float diff) { }
    }
}





