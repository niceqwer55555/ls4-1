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
    /// Leona W - Eclipse
    /// 获得护甲和魔抗加成，持续3秒
    /// 效果结束时对附近敌人造成魔法伤害
    /// </summary>
    public class Eclipse : ISpellScript
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

            // 护甲加成: 35/50/65/80
            // 魔抗加成: 35/50/65/80
            float[] armorBonus = { 35f, 50f, 65f, 80f };
            float[] mrBonus = { 35f, 50f, 65f, 80f };
            float armor = armorBonus[spell.CastInfo.SpellLevel - 1];
            float mr = mrBonus[spell.CastInfo.SpellLevel - 1];

            // 添加buff
            AddBuff("EclipseArmor", 3f, 1, spell, owner, owner);
            AddBuff("EclipseMR", 3f, 1, spell, owner, owner);

            // 3秒后对附近敌人造成伤害
            // 伤害公式: 60/95/130/165 + 0.4 AP
            float[] baseDamage = { 60f, 95f, 130f, 165f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            foreach (var unit in GetUnitsInRange(owner.Position, 300f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticle(owner, owner, "Eclipse_cas", owner.Position);
        }
    }
}
