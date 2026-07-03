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
    public class ToxicShotAttack : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var target = spell.CastInfo.Targets[0].Unit;

            // 被动技能：攻击时附加毒液伤害
            // 每秒魔法伤害: 6/12/18/24/30 (+10% AP)
            float[] damagePerSecond = { 6f, 12f, 18f, 24f, 30f };
            float ap = owner.Stats.AbilityPower.Total * 0.1f;
            float dps = damagePerSecond[spell.CastInfo.SpellLevel - 1] + ap;

            // 立即附加伤害: 10/20/30/40/50 (+30% AP)
            float[] initialDamage = { 10f, 20f, 30f, 40f, 50f };
            float initialAp = owner.Stats.AbilityPower.Total * 0.3f;
            float initial = initialDamage[spell.CastInfo.SpellLevel - 1] + initialAp;

            // 立即附加初始伤害
            target.TakeDamage(owner, initial, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLPERSIST, false);

            // 添加持续伤害buff (4秒)
            AddBuff("ToxicShot", 4f, 1, spell, target, owner);
        }
    }
}