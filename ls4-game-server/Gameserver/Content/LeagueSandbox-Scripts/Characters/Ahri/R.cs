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
    /// Ahri R - Spirit Rush
    /// Dashes forward and fires essence bolts at nearby enemies
    /// 3 charges, damage: 70/110/150 (+30% AP) per bolt
    /// </summary>
    public class AhriTumble : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
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
            var owner = spell.CastInfo.Owner;

            // Handle buff stacks (3 dashes)
            if (!owner.HasBuff("AhriTumble"))
            {
                AddBuff("AhriTumble", 8f, 3, spell, owner, owner);
            }
            else
            {
                var buff = owner.GetBuffWithName("AhriTumble");
                if (buff != null && buff.StackCount > 0)
                {
                    buff.DeactivateBuff();
                    AddBuff("AhriTumble", 8f, (byte)(buff.StackCount - 1), spell, owner, owner);
                }
                else
                {
                    return;
                }
            }

            // Dash to target position
            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var trueCoords = GetPointFromUnit(owner, 450f);
            TeleportTo(owner, trueCoords.X, trueCoords.Y);

            // Fire missiles at nearby enemies
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, true, null, targetPos);

            AddParticle(owner, owner, "Ahri_Base_R_Cas.troy", owner.Position);
            AddParticleTarget(owner, owner, "Ahri_Base_R_Flash.troy", owner);
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
    /// Ahri R - Spirit Rush Missile
    /// Damage: 70/110/150 (+30% AP) per bolt
    /// Prioritizes champions
    /// </summary>
    public class AhriTumbleMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle,
                CanHitSameTargetConsecutively = true,
            },
            IsDamagingSpell = true,
            TriggersSpellCasts = true,
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
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
            // Damage: 70/110/150 (+30% AP) - 3 level ultimate
            float[] baseDamage = { 70f, 110f, 150f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 0.30f;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            AddParticleTarget(owner, target, "Ahri_Base_R_tar.troy", target);
            missile.SetToRemove();
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
