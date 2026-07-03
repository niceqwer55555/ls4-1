using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.API;
using System.Numerics;

namespace Buffs
{
    internal class AlphaStrikeTeleport : IBuffGameScript
    {
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        private ObjAIBase Owner;
        private AttackableUnit Target;
        private Buff ThisBuff;

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            Owner = unit as ObjAIBase;
            ThisBuff = buff;
            SetStatus(unit, StatusFlags.NoRender, true);
            buff.SetStatusEffect(StatusFlags.Ghosted, true);
            buff.SetStatusEffect(StatusFlags.Targetable, false);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 0, SpellbookType.SPELLBOOK_CHAMPION, true);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 1, SpellbookType.SPELLBOOK_CHAMPION, true);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, true);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 3, SpellbookType.SPELLBOOK_CHAMPION, true);
            SealSpellSlot(Owner, SpellSlotType.SummonerSpellSlots, 0, SpellbookType.SPELLBOOK_SUMMONER, true);
            SealSpellSlot(Owner, SpellSlotType.SummonerSpellSlots, 1, SpellbookType.SPELLBOOK_SUMMONER, true);

            if (ownerSpell.CastInfo.Targets.Count > 0)
            {
                Target = ownerSpell.CastInfo.Targets[0].Unit;
                if (Target != null)
                {
                    ApiEventManager.OnMoveEnd.AddListener(this, Owner, OnMoveEnd, true);
                    FaceDirection(Target.Position, Owner, true);
                    var targetPos = GetPointFromUnit(Target, -125f);
                    ForceMovement(Owner, null, targetPos, 2200, 0, 0, 0);
                }
            }
        }

        public void OnMoveEnd(AttackableUnit unit)
        {
            SetStatus(unit, StatusFlags.NoRender, false);
            SetStatus(unit, StatusFlags.Ghosted, false);
            SetStatus(unit, StatusFlags.Targetable, true);
            SealSpellSlot(Owner, SpellSlotType.SummonerSpellSlots, 0, SpellbookType.SPELLBOOK_SUMMONER, false);
            SealSpellSlot(Owner, SpellSlotType.SummonerSpellSlots, 1, SpellbookType.SPELLBOOK_SUMMONER, false);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 0, SpellbookType.SPELLBOOK_CHAMPION, false);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 1, SpellbookType.SPELLBOOK_CHAMPION, false);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, false);
            SealSpellSlot(Owner, SpellSlotType.SpellSlots, 3, SpellbookType.SPELLBOOK_CHAMPION, false);
            Owner.RemoveBuffsWithName("AlphaStrikeTeleport");
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}