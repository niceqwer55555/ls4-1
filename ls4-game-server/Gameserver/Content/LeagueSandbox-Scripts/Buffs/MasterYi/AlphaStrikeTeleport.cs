using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

namespace Buffs
{
    internal class AlphaStrikeTeleport : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        private ObjAIBase Owner;

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            Owner = unit as ObjAIBase;
            if (Owner == null)
            {
                Owner = ownerSpell.CastInfo.Owner;
            }
            SetStatus(unit, StatusFlags.NoRender, true);
            SetStatus(unit, StatusFlags.Ghosted, true);
            SetStatus(unit, StatusFlags.Targetable, false);
            Owner.SetDashingState(true);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            if (unit is ObjAIBase ai)
            {
                if (Spells.AlphaStrike._target == null || Spells.AlphaStrike._target.IsDead)
                {
                    TeleportTo(ai, Spells.AlphaStrike._startPos.X, Spells.AlphaStrike._startPos.Y);
                }
            }
            Owner.SetDashingState(false);
            SetStatus(unit, StatusFlags.NoRender, false);
            SetStatus(unit, StatusFlags.Ghosted, false);
            SetStatus(unit, StatusFlags.Targetable, true);
            SetStatus(unit, StatusFlags.CanAttack, true);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}