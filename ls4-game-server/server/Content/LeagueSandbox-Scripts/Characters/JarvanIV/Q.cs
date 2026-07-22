using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;

namespace Spells
{
    /// <summary>
    /// Jarvan IV Q - Dragon Strike
    /// 长矛穿刺，击中E旗帜时突进到旗帜位置
    /// </summary>
    public class JarvanIVQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 物理伤害: 70/115/160/205/250 (+1.2 bonus AD)
            float[] baseDamage = { 70f, 115f, 160f, 205f, 250f };
            float bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + bonusAd * 1.2f;

            foreach (var unit in GetUnitsInRange(targetPos, 250f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 检查是否击中E旗帜，如果是则突进到旗帜位置
            var flagBuff = owner.GetBuffWithName("JarvanIVEFlag");
            if (flagBuff != null)
            {
                var flagPos = new Vector2(flagBuff.SourceUnit.Position.X, flagBuff.SourceUnit.Position.Y);
                var dist = Vector2.Distance(targetPos, flagPos);
                if (dist < 300f)
                {
                    TeleportTo(owner, flagPos.X, flagPos.Y);
                }
            }

            AddParticle(owner, owner, "JarvanIVQ_cas", targetPos);
        }
    }
}
