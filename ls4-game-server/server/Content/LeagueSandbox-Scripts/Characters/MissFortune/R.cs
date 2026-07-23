using System;
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
    /// Miss Fortune R - Bullet Time
    /// 发射锥形弹雨，对范围内敌人造成大量物理伤害
    /// </summary>
    public class MissFortuneBulletTime : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var castPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            FaceDirection(castPos, owner, true);

            // 伤害计算
            float baseDamage = 100f + (spell.CastInfo.SpellLevel * 150f);
            float adBonus = owner.Stats.AttackDamage.Total * 1.2f;
            float damage = baseDamage + adBonus;

            // 锥形范围
            float range = 600f;
            float coneAngle = 60f;
            var direction = Vector2.Normalize(castPos - owner.Position);

            foreach (var unit in GetUnitsInRange(owner.Position, range, true))
            {
                if (unit.Team != owner.Team)
                {
                    var toUnit = Vector2.Normalize(unit.Position - owner.Position);
                    var dot = Vector2.Dot(direction, toUnit);
                    float angle = (float)Math.Acos(dot) * (180f / (float)Math.PI);

                    if (angle <= coneAngle / 2)
                    {
                        unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL,
                            DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                    }
                }
            }

            // 粒子效果
            AddParticle(owner, owner, "MissFortune_R_cas", owner.Position, lifetime: 1.0f);
        }

        public void OnSpellPostCast(Spell spell) { }
        public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}