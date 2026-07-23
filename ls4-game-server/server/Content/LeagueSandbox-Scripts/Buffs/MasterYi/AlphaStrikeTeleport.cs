using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using System.Numerics;

namespace Buffs
{
    /// <summary>
    /// AlphaStrike teleport buff - makes Yi untargetable during Q animation.
    /// This buff is no longer the primary mechanism (Q.cs handles teleport directly),
    /// but kept for compatibility if referenced by other systems.
    /// </summary>
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
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
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
