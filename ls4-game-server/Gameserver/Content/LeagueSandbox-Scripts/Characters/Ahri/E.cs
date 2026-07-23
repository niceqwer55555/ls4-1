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

namespace Spells
{
    /// <summary>
    /// Ahri E - Charm
    /// Fires a kiss that damages and charms an enemy
    /// Damage: 60/90/120/150/180 (+35% AP)
    /// Charm duration: 1/1.25/1.5/1.75/2 seconds
    /// </summary>
    public class AhriSeduce : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
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
            var endPos = GetPointFromUnit(spell.CastInfo.Owner, 1075f);
            SpellCast(spell.CastInfo.Owner, 4, SpellSlotType.ExtraSlots, endPos, endPos, true, Vector2.Zero);
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
    /// Ahri E - Charm Missile
    /// </summary>
    public class AhriSeduceMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            }
        };

        private ObjAIBase _owner;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            _owner = owner;
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

            // Damage: 60/90/120/150/180 (+35% AP)
            float[] baseDamage = { 60f, 90f, 120f, 150f, 180f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 0.35f;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // Charm: 1/1.25/1.5/1.75/2 seconds
            float[] charmDuration = { 1.0f, 1.25f, 1.5f, 1.75f, 2.0f };
            float charmTime = charmDuration[spell.CastInfo.SpellLevel - 1];

            // Apply charm - force walk toward Ahri
            FaceDirection(owner.Position, target);
            var walkTarget = GetPointFromUnit(target, 300f);
            var targetAI = target as ObjAIBase;
            if (targetAI != null)
            {
                targetAI.SetTargetUnit(null, true);
            }
            ForceMovement(target, "RUN", walkTarget, 10, 0, 0, 0);

            // Stun/charm buff
            AddBuff("AhriSeduce", charmTime, 1, spell, target, owner);

            AddParticleTarget(owner, target, "Ahri_Seduce_tar.troy", target);
            missile.SetToRemove();

            // Passive stack for healing
            AddBuff("AhriSoulCrusherCounter", float.MaxValue, 1, spell, owner, owner);

            if (owner.GetBuffWithName("AhriSoulCrusherCounter") != null
                && owner.GetBuffWithName("AhriSoulCrusherCounter").StackCount >= 9)
            {
                PerformHeal(owner, spell, owner);
                CreateTimer(0.1f, () => { RemovePassiveStacks(9); });
            }
        }

        private void RemovePassiveStacks(int count)
        {
            int removed = 0;
            foreach (var buff in _owner.GetBuffsWithName("AhriSoulCrusherCounter"))
            {
                if (removed < count)
                {
                    removed++;
                    buff.DeactivateBuff();
                }
            }
        }

        private void PerformHeal(ObjAIBase owner, Spell spell, AttackableUnit target)
        {
            var ap = owner.Stats.AbilityPower.Total * spell.SpellData.MagicDamageCoefficient;
            float[] healPerLevel = { 15f, 30f, 45f, 60f, 75f }; float healthGain = healPerLevel[spell.CastInfo.SpellLevel - 1] + (float)ap;
            if (target.HasBuff("HealCheck"))
            {
                healthGain *= 0.5f;
            }
            var newHealth = target.Stats.CurrentHealth + healthGain;
            target.Stats.CurrentHealth = Math.Min(newHealth, target.Stats.HealthPoints.Total);
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

