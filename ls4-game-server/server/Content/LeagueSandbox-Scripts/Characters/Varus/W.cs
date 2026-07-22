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
    /// Varus W - Blighted Quiver (Passive)
    /// 被动技能：Varus的普通攻击造成额外魔法伤害并叠加Blight(诅咒)
    /// 其他技能会引爆Blight，造成基于目标最大生命值的魔法伤害
    /// </summary>
    public class VarusW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 每层Blight伤害: 2/2.75/3.5/4.25/5% 最大生命值 (+0.02% AP)
            float[] blightDamagePercent = { 0.02f, 0.0275f, 0.035f, 0.0425f, 0.05f };
            float ap = owner.Stats.AbilityPower.Total * 0.0002f;
            float percent = blightDamagePercent[spell.CastInfo.SpellLevel - 1] + ap;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target != null && target.IsDead)
            {
                // 引爆Blight造成基于最大生命值的伤害
                float damage = target.Stats.HealthPoints.Total * percent;
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            }
        }
    }
}
