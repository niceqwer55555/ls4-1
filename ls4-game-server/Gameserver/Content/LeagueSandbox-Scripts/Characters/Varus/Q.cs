using System;
using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    /// <summary>
    /// Varus Q - Piercing Arrow
    /// Fires a long-range arrow that deals physical damage
    /// and pierces through enemies (15% reduced damage per hit after first)
    /// </summary>
    public class VarusQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var distance = Vector2.Distance(owner.Position, targetPos);
            FaceDirection(targetPos, owner);

            if (distance >= 1475.0)
            {
                targetPos = GetPointFromUnit(owner, 1475.0f);
            }

            SpellCast(owner, 0, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
        }
    }

    /// <summary>
    /// Varus Q Missile - piercing arrow
    /// Damage: 10/47/83/120/157 (+1.0 AD) to 15/70/125/180/235 (+1.6 AD) based on charge time
    /// </summary>
    public class VarusQMissile : ISpellScript
    {
        private ObjAIBase Varus;
        private List<AttackableUnit> UnitsHit = new List<AttackableUnit>();

        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters { Type = MissileType.Circle },
            IsDamagingSpell = true,
            TriggersSpellCasts = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            UnitsHit.Clear();
            Varus = owner = spell.CastInfo.Owner as Champion;
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            if (UnitsHit.Contains(target)) return;
            UnitsHit.Add(target);

            var owner = spell.CastInfo.Owner;
            // Simplified: use mid-range values
            // 10/47/83/120/157 (+1.0 AD)
            float[] minBaseDamage = { 10f, 47f, 83f, 120f, 157f };
            float damage = minBaseDamage[spell.CastInfo.SpellLevel - 1] + owner.Stats.AttackDamage.Total;

            // 15% reduced damage per enemy hit after first (minimum 33%)
            for (int i = 1; i < UnitsHit.Count; i++)
            {
                damage *= 0.85f;
            }
            damage = Math.Max(damage, minBaseDamage[spell.CastInfo.SpellLevel - 1] * 0.33f);

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddParticleTarget(owner, target, "Varus_Base_Q_Tar.troy", target);
        }
    }
}
