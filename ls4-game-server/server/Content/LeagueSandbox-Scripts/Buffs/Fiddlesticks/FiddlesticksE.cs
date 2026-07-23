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
    /// Fiddlesticks E - Dark Wind
    /// 沉默效果
    /// </summary>
    internal class FiddlesticksE : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.SILENCE,
            BuffAddType = BuffAddType.REPLACE_EXISTING,
            IsHidden = false
        };

        Particle silence;
public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.SetStatusEffect(StatusFlags.Silenced, true);
            silence = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Fiddlesticks_E_Icon", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(silence);
            unit.SetStatusEffect(StatusFlags.Silenced, false);
        }
    }
}




