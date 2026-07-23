using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using static GameServerCore.Content.HashFunctions;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    public class EzrealMysticShot : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        private ObjAIBase _owner;
        private Spell _spell;
        private float _bonusAd = 0;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            _owner = owner;
            _spell = spell;
            ApiEventManager.OnUpdateStats.AddListener(this, owner, OnStatsUpdate, false);
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddParticleTarget(owner, owner, "ezreal_bow", owner, bone: "L_HAND");
        }

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var distance = Vector2.Distance(owner.Position, targetPos);
            FaceDirection(targetPos, owner);

            if (distance > 1200.0)
            {
                targetPos = GetPointFromUnit(owner, 1150.0f);
            }

            if (owner.SkinID == 5)
            {
                SpellCast(owner, 3, SpellSlotType.ExtraSlots, targetPos, targetPos, true, Vector2.Zero);
            }
            else
            {
                SpellCast(owner, 0, SpellSlotType.ExtraSlots, targetPos, targetPos, true, Vector2.Zero);
            }
        }

        private void OnStatsUpdate(AttackableUnit _unit, float diff)
        {
            float bonusAd = _owner.Stats.AttackDamage.Total * _spell.SpellData.AttackDamageCoefficient;
            if (_bonusAd != bonusAd)
            {
                _bonusAd = bonusAd;
                SetSpellToolTipVar(_owner, 2, bonusAd, SpellbookType.SPELLBOOK_CHAMPION, 0, SpellSlotType.SpellSlots);
            }
        }
    }

    public class EzrealMysticShotPulseMissile : EzrealMysticShotMissile
    {
    }

    /// <summary>
    /// Ezreal Q Missile - Mystic Shot
    /// Damage: 35/55/75/95/115 (+110% AD) (+40% AP)
    /// </summary>
    public class EzrealMysticShotMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters { Type = MissileType.Circle },
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            // Damage: 35/55/75/95/115 (+110% AD) (+40% AP)
            float[] baseDamage = { 35f, 55f, 75f, 95f, 115f };
            var ad = owner.Stats.AttackDamage.Total * 1.1f;
            var ap = owner.Stats.AbilityPower.Total * 0.4f;
            var damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad + ap;

            IEventSource source = owner.SkinID == 5
                ? new AbilityInfo(266740993, HashString("EzrealMysticShot"))
                : (IEventSource)new AbilityInfo(3693728257, HashString("EzrealMysticShot"));

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false, source);

            for (byte i = 0; i < 4; i++)
            {
                owner.Spells[i].LowerCooldown(1);
            }

            AddParticleTarget(owner, target, "Ezreal_mysticshot_tar", target);
            missile.SetToRemove();
        }
    }
}
