using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System;
using System.Linq;
using GameServerCore;

namespace Spells
{
    /// <summary>
    /// Ahri W - Fox Fire
    /// Releases 3 fox fires that lock on to nearby enemies
    /// First fire: 40/65/90/115/140 (+40% AP) magic damage
    /// Additional fires: 12/19.5/27/34.5/42 (+12% AP) magic damage each
    /// </summary>
    public class AhriFoxFire : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        private ObjAIBase _owner;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            _owner = owner;
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
        }

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddParticle(owner, owner, "Ahri_FoxFire_cas.troy", owner.Position);

            var units = GetUnitsInRange(owner.Position, 1000, true)
                .Where(x => x.Team != owner.Team && x != owner)
                .Take(3)
                .ToList();

            if (units.Count > 0)
            {
                foreach (var enemyTarget in units)
                {
                    SpellCast(owner, 3, SpellSlotType.ExtraSlots, true, enemyTarget, Vector2.Zero);
                }
            }
            else
            {
                // No targets - cast on self (fires will appear but deal no damage)
                SpellCast(owner, 3, SpellSlotType.ExtraSlots, true, owner, Vector2.Zero);
            }
        }

        public void OnSpellChannel(Spell spell)
        {
        }

        public void OnSpellChannelCancel(Spell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(Spell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }

    /// <summary>
    /// Ahri W - Fox Fire Missile
    /// First fox fire: 40/65/90/115/140 (+40% AP)
    /// Additional: 12/19.5/27/34.5/42 (+12% AP)
    /// </summary>
    public class AhriFoxFireMissileTwo : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            }
        };

        private int _hitCount = 0;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            _hitCount = 0;
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
        }

        public void OnSpellPostCast(Spell spell)
        {
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var spellLevel = spell.CastInfo.SpellLevel;

            float damage;
            if (_hitCount == 0)
            {
                // First fox fire - full damage
                float[] baseDamage = { 40f, 65f, 90f, 115f, 140f };
                damage = baseDamage[spellLevel - 1] + owner.Stats.AbilityPower.Total * 0.4f;
            }
            else
            {
                // Additional fox fires - reduced damage
                float[] reducedDamage = { 12f, 19.5f, 27f, 34.5f, 42f };
                damage = reducedDamage[spellLevel - 1] + owner.Stats.AbilityPower.Total * 0.12f;
            }

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddParticleTarget(owner, target, "Ahri_FoxFire_tar.troy", target);
            missile.SetToRemove();
            _hitCount++;
        }

        public void OnSpellChannel(Spell spell)
        {
        }

        public void OnSpellChannelCancel(Spell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(Spell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
