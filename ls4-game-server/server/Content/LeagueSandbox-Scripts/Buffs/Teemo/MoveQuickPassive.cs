using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace Buffs
{
    /// <summary>
    /// Teemo W Passive - Move Quick passive buff.
    /// Grants 10/14/18/22/26% bonus move speed (always active while W is learned).
    /// </summary>
    internal class MoveQuickPassive : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HASTE,
            BuffAddType = BuffAddType.REPLACE_EXISTING,
            IsHidden = true
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            float[] passiveMoveSpeed = { 0.10f, 0.14f, 0.18f, 0.22f, 0.26f };
            float passiveSpeed = passiveMoveSpeed[ownerSpell.CastInfo.SpellLevel - 1];

            StatsModifier.MoveSpeed.PercentBonus = passiveSpeed;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.RemoveStatModifier(StatsModifier);
        }
    }
}
