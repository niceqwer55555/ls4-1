using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Buffs
{
    /// <summary>
    /// Zyra R - Strangle
    /// 击飞效果
    /// </summary>
    internal class ZyraR : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.KNOCKUP,
            BuffAddType = BuffAddType.REPLACE_EXISTING,
            IsHidden = false
        };

        Particle knockup;
public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.SetStatusEffect(StatusFlags.Knockup, true);
            knockup = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Zyra_R_Icon", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(knockup);
            unit.SetStatusEffect(StatusFlags.Knockup, false);
        }
    }
}




