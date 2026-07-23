using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using System.Numerics;
using System.Collections.Generic;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings;

namespace Spells
{
    /// <summary>
    /// MasterYi Q - Alpha Strike
    /// Teleports to target and deals damage to up to 4 nearby enemies.
    /// </summary>
    public class AlphaStrike : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        private AttackableUnit _target;
        private Vector2 _startPos;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            _target = target;
            _startPos = owner.Position;
        }

        public void OnSpellCast(Spell spell)
        {
        }

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null || _target == null || _target.IsDead)
            {
                return;
            }

            var spellLevel = spell.CastInfo.SpellLevel;
            // Base damage: 25/60/95/130/165
            float[] baseDamageArr = { 25f, 60f, 95f, 130f, 165f };
            var baseDamage = baseDamageArr[spellLevel - 1];
            var adRatio = owner.Stats.AttackDamage.Total * 1.0f;

            // Make owner untargetable and invisible during Q
            SetStatus(owner, StatusFlags.NoRender, true);
            SetStatus(owner, StatusFlags.Ghosted, true);
            SetStatus(owner, StatusFlags.Targetable, false);

            // Teleport to target
            TeleportTo(owner, _target.Position.X, _target.Position.Y);

            // Cast effect particle
            AddParticleTarget(owner, owner, "MasterYi_Base_Q_Cas.troy", owner, 3f);

            // Find up to 4 nearby enemies (including primary target)
            var units = GetUnitsInRange(_target.Position, 600f, true);
            var hitTargets = new List<AttackableUnit>();
            var primaryHit = false;

            foreach (var unit in units)
            {
                if (unit.Team != owner.Team && unit != owner && !(unit is ObjBuilding || unit is BaseTurret) && !unit.IsDead)
                {
                    if (unit == _target)
                    {
                        primaryHit = true;
                    }

                    hitTargets.Add(unit);
                    if (hitTargets.Count >= 4)
                    {
                        break;
                    }
                }
            }

            // If primary target was not in range list, add it
            if (!primaryHit && !_target.IsDead && _target.Team != owner.Team)
            {
                hitTargets.Insert(0, _target);
                if (hitTargets.Count > 4)
                {
                    hitTargets.RemoveAt(hitTargets.Count - 1);
                }
            }

            // If no targets found, just hit the primary target
            if (hitTargets.Count == 0)
            {
                hitTargets.Add(_target);
            }

            // Deal damage to each target
            foreach (var target in hitTargets)
            {
                var damage = baseDamage + adRatio;

                // Extra damage to minions: 50
                if (target is Minion)
                {
                    damage += 50f;
                }

                AddParticleTarget(owner, target, "MasterYi_Base_Q_Tar.troy", target, 1f);
                AddParticleTarget(owner, target, "MasterYi_Base_Q_Tar_Mark.troy", target, 1f);
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            }

            // After a short delay, teleport back and remove untargetable
            CreateTimer(0.25f, () =>
            {
                // Teleport back to start position if target is dead, otherwise stay near target
                if (_target == null || _target.IsDead)
                {
                    TeleportTo(owner, _startPos.X, _startPos.Y);
                }

                SetStatus(owner, StatusFlags.NoRender, false);
                SetStatus(owner, StatusFlags.Ghosted, false);
                SetStatus(owner, StatusFlags.Targetable, true);
            });
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
