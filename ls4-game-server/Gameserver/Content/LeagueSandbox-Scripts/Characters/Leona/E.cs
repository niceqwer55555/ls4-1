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
    /// Leona E - Zenith Blade
    /// 向前释放一道光刃，对敌人造成伤害并眩晕
    /// 闪烁到目标位置
    /// </summary>
    public class ZenithBlade : ISpellScript
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

            // 伤害公式: 70/115/160/205/250 + 0.5 AP
            float[] baseDamage = { 70f, 115f, 160f, 205f, 250f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对光刃路径上的敌人造成伤害
            foreach (var unit in GetUnitsInRange(owner.Position, 900f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 眩晕0.5秒
                    AddBuff("Stun", 0.5f, 1, spell, unit, owner);
                }
            }

            // 闪烁到目标位置
            TeleportTo(owner, targetPos.X, targetPos.Y);

            AddParticle(owner, owner, "ZenithBlade_cas", owner.Position);
        }
    }
}
