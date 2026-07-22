using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Diana Q - Crescent Strike
    /// 释放一道月能光束，对弧形范围内的敌人造成魔法伤害
    /// </summary>
    public class CrescentStrike : ISpellScript
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

            // 伤害公式: 60/95/130/165/200 + 0.7 AP
            float[] baseDamage = { 60f, 95f, 130f, 165f, 200f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 弧形攻击
            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var direction = Vector2.Normalize(targetPos - owner.Position);

            foreach (var unit in GetUnitsInRange(owner.Position, 900f, true))
            {
                if (unit.Team != owner.Team)
                {
                    var toUnit = Vector2.Normalize(unit.Position - owner.Position);
                    var dot = direction.X * toUnit.X + direction.Y * toUnit.Y;
                    if (dot > 0.5f)
                    {
                        unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                        // 添加Moonlight debuff
                        AddBuff("Moonlight", 3f, 1, spell, unit, owner);
                    }
                }
            }

            AddParticle(owner, owner, "CrescentStrike_cas", owner.Position);
        }
    }
}
