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
    /// Lux Q - Light Binding
    /// 释放一个光球，束缚最多两个敌人
    /// </summary>
    public class LightBinding : ISpellScript
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

            // 伤害公式: 60/110/160/210/260 + 0.7 AP
            float[] baseDamage = { 60f, 110f, 160f, 210f, 260f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对命中的敌人造成伤害和束缚
            int hitCount = 0;
            foreach (var unit in GetUnitsInRange(owner.Position, 1175f, true))
            {
                if (unit.Team != owner.Team && hitCount < 2)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 第一个目标束缚2秒，第二个1秒
                    float stunDuration = hitCount == 0 ? 2f : 1f;
                    AddBuff("Stun", stunDuration, 1, spell, unit, owner);
                    hitCount++;
                }
            }

            AddParticle(owner, owner, "LightBinding_cas", owner.Position);
        }
    }
}
