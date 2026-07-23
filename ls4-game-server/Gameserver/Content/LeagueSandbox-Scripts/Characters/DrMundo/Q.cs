using System;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerLib.GameObjects.AttackableUnits;
using GameServerCore;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    /// <summary>
    /// DrMundo Q - Infected Cleaver (cast)
    /// Self-damage: 40/50/60/70/80 health cost
    /// </summary>
    public class InfectedCleaverMissileCast : ISpellScript
    {
        private ObjAIBase Owner;

        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            Owner = owner;
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
            var owner = spell.CastInfo.Owner as Champion;
            var targetPos = GetPointFromUnit(owner, 975f);
            // Self damage: 40/50/60/70/80
            float[] selfDamage = { 40f, 50f, 60f, 70f, 80f };
            float SelfDamage = selfDamage[spell.CastInfo.SpellLevel - 1];

            FaceDirection(targetPos, owner);
            SpellCast(owner, 0, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
            owner.StopMovement();
            owner.SetTargetUnit(null);
            if (owner.Stats.CurrentHealth > SelfDamage)
            {
                owner.Stats.CurrentHealth -= SelfDamage;
            }
            else
            {
                owner.Stats.CurrentHealth = 1f;
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
            // Passive: Mundo regenerates 0.3% max HP per second
            if (Owner != null)
            {
                Owner.Stats.HealthRegeneration.FlatBonus = (Owner.Stats.HealthPoints.Total / 100f) * 0.3f;
            }
        }
    }

    /// <summary>
    /// DrMundo Q - Infected Cleaver (missile)
    /// Damage: 15/18/21/23/25% of target current HP (magic damage)
    /// Minimum damage: 80/130/180/230/280
    /// Heal on hit: 50/60/70/80/90
    /// </summary>
    public class InfectedCleaverMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        private void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector arg4)
        {
            var owner = spell.CastInfo.Owner;
            var spellLevel = owner.GetSpell("InfectedCleaverMissileCast").CastInfo.SpellLevel;

            // Damage: 15/18/21/23/25% of target current HP
            float[] hpPercent = { 0.15f, 0.18f, 0.21f, 0.23f, 0.25f };
            var damage = target.Stats.CurrentHealth * hpPercent[spellLevel - 1];

            // Minimum damage: 80/130/180/230/280
            float[] minDamage = { 80f, 130f, 180f, 230f, 280f };
            float minimunDamage = minDamage[spellLevel - 1];
            if (damage < minimunDamage)
            {
                damage = minimunDamage;
            }

            // Heal: 50/60/70/80/90
            float[] healAmount = { 50f, 60f, 70f, 80f, 90f };
            float Heal = healAmount[spellLevel - 1];

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + Heal, owner.Stats.HealthPoints.Total);
            AddParticleTarget(owner, null, "dr_mundo_infected_cleaver_tar.troy", target);
            AddBuff("InfectedCleaverMissile", 2f, 1, spell, target, owner);

            missile.SetToRemove();
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

