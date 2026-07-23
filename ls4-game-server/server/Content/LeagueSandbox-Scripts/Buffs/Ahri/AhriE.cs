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
    internal class AhriE : IBuffGameScript
    {
                public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.CHARM,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        Particle charm;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            unit.SetStatusEffect(StatusFlags.Charmed, true);
            charm = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Ahri_E_Icon", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(charm);
            unit.SetStatusEffect(StatusFlags.Charmed, false);
        }
    }
}







