using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Miss Fortune Q - Double Up
    /// 发射一颗子弹伤害一个目标，然后弹射到后面的敌人
    /// </summary>
    public class MissFortuneBulletMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;

            // 基础伤害
            float baseDamage = 25f + (spell.CastInfo.SpellLevel * 20f);
            float adBonus = owner.Stats.AttackDamage.Total * 0.85f;
            float damage = baseDamage + adBonus;

            // 造成伤害
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL,
                DamageSource.DAMAGE_SOURCE_SPELL, false);

            AddParticleTarget(owner, target, "MissFortune_Q_hit", target, lifetime: 0.5f);

            // 获取弹射目标
            var castPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var direction = Vector2.Normalize(castPos - owner.Position);

            AttackableUnit bounceTarget = null;
            float maxDistance = 500f;

            foreach (var unit in GetUnitsInRange(target.Position, maxDistance, true))
            {
                if (unit.Team != owner.Team && unit != target && unit is ObjAIBase)
                {
                    var toUnit = Vector2.Normalize(unit.Position - target.Position);
                    var dot = Vector2.Dot(direction, toUnit);

                    if (dot > 0.5f)
                    {
                        bounceTarget = unit;
                        break;
                    }
                }
            }

            if (bounceTarget != null)
            {
                float bounceDamage = damage * 1.2f;
                bounceTarget.TakeDamage(owner, bounceDamage, DamageType.DAMAGE_TYPE_PHYSICAL,
                    DamageSource.DAMAGE_SOURCE_SPELL, false);

                AddParticleTarget(owner, bounceTarget, "MissFortune_Q_hit_secondary", bounceTarget, lifetime: 0.5f);
            }
        }

        public void OnSpellPostCast(Spell spell) { }
        public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}