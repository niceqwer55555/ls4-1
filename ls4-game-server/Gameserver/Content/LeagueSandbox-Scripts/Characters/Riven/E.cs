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
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    /// <summary>
    /// Riven E - Valor
    /// Dashes toward cursor and gains a shield that absorbs 90/120/150/180/210 (+100% bonus AD) damage for 1.5s
    /// </summary>
    public class RivenFeint : ISpellScript
    {
        Spell Feint;
        ObjAIBase Riven;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Feint = spell;
            Riven = owner = spell.CastInfo.Owner as Champion;
            Riven.CancelAutoAttack(false, false);
        }

        public void OnSpellPostCast(Spell spell)
        {
            PlayAnimation(Riven, "Spell3", 0.25f);
            AddBuff("RivenFeint", 1.5f, 1, spell, Riven, Riven);
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            FaceDirection(spellPos, Riven, true);
            var trueCoords = GetPointFromUnit(Riven, spell.SpellData.CastRangeDisplayOverride);
            ForceMovement(Riven, null, trueCoords, 1450, 0, 0, 0, movementOrdersFacing: ForceMovementOrdersFacing.KEEP_CURRENT_FACING);
        }
    }
}
