using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    public class DarkBinding : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            SpellDamageRatio = 0.9f,
            AutoTargetDamageByLevel = new[] { 80.0f, 135.0f, 190.0f, 245.0f, 300.0f },
            AutoCooldownByLevel = new[] { 11.0f, 11.0f, 11.0f, 11.0f, 11.0f }
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
            var owner = spell.CastInfo.Owner;
            var ownerSkinID = owner.SkinID;
            var targetPos = spell.CastInfo.TargetPosition;

            var startPos = new Vector3(owner.Position.X, owner.Position.Y, 0);
            var direction = Vector3.Normalize(new Vector3(targetPos.X, targetPos.Y, 0) - startPos);
            var endPos = startPos + direction * 1175f;

            AddParticleTarget(owner, owner, "DarkBinding_mis.troy", owner, 1f, 1, "C_BuffBone_Glb_Center_Loc");
            PlaySound("Morgana_Q_Cast", owner);

            spell.CreateSpellSector(new SectorParameters
            {
                Length = 1175f,
                Width = 70f,
                Type = SectorType.Area,
                SingleTick = true,
                CanHitSameTarget = false
            });

            var requiredMana = owner.Stats.ManaPoints.Total * 0.5f;
            if (owner.Stats.CurrentMana >= requiredMana)
            {
                AddParticleTarget(owner, owner, "DarkBinding_mis_empowered.troy", owner, 1f, 1, "C_BuffBone_Glb_Center_Loc");
                owner.Stats.CurrentMana -= requiredMana;
            }

            PlayAnimation(owner, "Spell1", 0.8f);
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            if (owner != target)
            {
                var damage = ScriptMetadata.AutoTargetDamageByLevel[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * ScriptMetadata.SpellDamageRatio;
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                AddBuff("DarkBinding", 2.0f, 1, spell, target, owner);

                AddParticle(owner, target, "DarkBinding_tar.troy", target.Position);
                PlaySound("Morgana_Q_Hit", target);
            }
        }

        public void OnSpellChannel(Spell spell)
        {
        }

        public void OnSpellChannelCancel(Spell spell, ChannelingStopSource reason)
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