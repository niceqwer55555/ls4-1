using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Buffs
{
    internal class RengarQ : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HASTE,
            BuffAddType = BuffAddType.RENEW_EXISTING,
            IsHidden = false
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            if (ownerSpell != null)
            {
                float[] attackSpeed = { 0.3f, 0.35f, 0.4f, 0.45f, 0.5f };
                float speed = attackSpeed[ownerSpell.CastInfo.SpellLevel - 1];
                StatsModifier.AttackSpeed.PercentBonus += speed;
                unit.AddStatModifier(StatsModifier);
            }
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.RemoveStatModifier(StatsModifier);
        }
    }
}