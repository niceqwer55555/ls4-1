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
    /// Lulu Q - Glitterlance
    /// 发射穿刺飞弹，造成魔法伤害并减速敌人
    /// </summary>
    public class Glitterlance : ISpellScript
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

            // 伤害公式: 70/105/140/175/210 + 0.5 AP
            float[] baseDamage = { 70f, 105f, 140f, 175f, 210f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对路径上的敌人造成伤害
            foreach (var unit in GetUnitsInRange(owner.Position, 925f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 减速80% (衰减2秒)
            foreach (var unit in GetUnitsInRange(owner.Position, 925f, true))
            {
                if (unit.Team != owner.Team)
                {
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "Glitterlance_cas", owner.Position);
        }
    }
}
