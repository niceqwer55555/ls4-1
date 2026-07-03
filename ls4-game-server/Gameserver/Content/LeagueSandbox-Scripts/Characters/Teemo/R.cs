using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace Spells
{
    public class NoxiousTrap : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.MISSILE_CIRCLE
            }
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var targetPos = spell.CastInfo.TargetPositionEnd;

            // 陷阱范围: 400/650/900
            float[] trapRange = { 400f, 650f, 900f };
            float range = trapRange[spell.CastInfo.SpellLevel - 1];

            // 添加蘑菇陷阱buff到目标位置
            AddBuff("NoxiousTrap", 300f, 1, spell, owner, owner);
        }

        public void ApplyEffects(AttackableUnit target, Spell spell, SpellMissile missile)
        {
            var owner = spell.CastInfo.Owner;

            // 总魔法伤害: 200/325/450 (+50% AP)
            float[] totalDamage = { 200f, 325f, 450f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float damage = totalDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 造成伤害
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 减速 30/40/50%
            float[] slowAmount = { 0.30f, 0.40f, 0.50f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];

            var slowMod = new StatsModifier();
            slowMod.MoveSpeed.PercentBonus = -slow;
            target.AddStatModifier(slowMod);

            AddBuff("Slow", 4f, 1, spell, target, owner);

            missile.SetToRemove();
        }
    }
}