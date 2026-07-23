using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Spells
{
    /// <summary>
    /// Twitch W - Venom Cask
    /// Throws a cask that explodes, applying 2 stacks of Deadly Venom
    /// and slowing enemies by 25/30/35/40/45% for 3 seconds
    /// </summary>
    public class TwitchVenomCask : ISpellScript
    {
        ObjAIBase Twitch;
        Vector2 Truecoords;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false
        };

        public void OnSpellPostCast(Spell spell)
        {
            Twitch = spell.CastInfo.Owner as Champion;
            var Cursor = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var current = new Vector2(Twitch.Position.X, Twitch.Position.Y);
            var distance = Cursor - current;
            if (distance.Length() > 1200)
            {
                distance = Vector2.Normalize(distance);
                var range = distance * 1200;
                Truecoords = current + range;
            }
            else
            {
                Truecoords = Cursor;
            }
            SpellCast(Twitch, 1, SpellSlotType.ExtraSlots, Truecoords, Truecoords, true, Vector2.Zero);
        }
    }

    /// <summary>
    /// Twitch W Missile - applies venom and slow on landing
    /// </summary>
    public class TwitchVenomCaskMissile : ISpellScript
    {
        private Spell Cask;
        private ObjAIBase Twitch;
        private SpellMissile Missile;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            IsDamagingSpell = false,
            TriggersSpellCasts = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Cask = spell;
            Twitch = owner = spell.CastInfo.Owner as Champion;
            Missile = spell.CreateSpellMissile(new MissileParameters { Type = MissileType.Circle, OverrideEndPosition = end });
            ApiEventManager.OnSpellMissileEnd.AddListener(this, Missile, OnMissileEnd, true);
        }

        public void OnMissileEnd(SpellMissile missile)
        {
            // Apply slow and venom to all enemies in the area
            var spellLevel = Twitch.Spells[1].CastInfo.SpellLevel;
            float[] slowPercent = { 25f, 30f, 35f, 40f, 45f };

            foreach (var unit in GetUnitsInRange(missile.Position, 275f, true))
            {
                if (unit.Team != Twitch.Team)
                {
                    // Apply 2 stacks of Deadly Venom
                    AddBuff("TwitchDeadlyVenom", 6f, 2, Cask, unit, Twitch);
                    // Slow
                    AddBuff("TwitchVenomCask", 3f, 1, Cask, unit, Twitch);
                }
            }

            AddParticle(Twitch, null, "Twitch_Base_W_Aoe.troy", missile.Position);

            // Spawn a visible minion at the landing point for the AoE effect
            Minion W = AddMinion(Twitch, "TestCubeRender", "TestCubeRender", missile.Position, Twitch.Team, Twitch.SkinID, true, false);
            AddBuff("TwitchVenomCask", 3f, 1, Cask, W, Twitch, false);
        }
    }
}

