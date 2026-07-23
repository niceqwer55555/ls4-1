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
    internal class VeigarEventHorizon : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.STUN,
            BuffAddType = BuffAddType.RENEW_EXISTING,
            IsHidden = false
        };

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            if (ownerSpell != null)
            {
                AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Veigar_Base_E_tar", unit, 1f);
            }
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
        }
    }
}