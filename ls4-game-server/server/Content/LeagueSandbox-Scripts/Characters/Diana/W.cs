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
    /// Diana W - Pale Cascade
    /// 召出三颗护体法球，在命中敌人后爆炸，对他们造成魔法伤害
    /// 同时获得护盾
    /// </summary>
    public class PaleCascade : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 伤害公式: 22/34/46/58/70 + 0.2 AP
            float[] baseDamage = { 22f, 34f, 46f, 58f, 70f };
            float ap = owner.Stats.AbilityPower.Total * 0.2f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 护盾公式: 40/55/70/85/100 + 0.3 AP
            float[] shieldAmount = { 40f, 55f, 70f, 85f, 100f };
            float shieldAp = owner.Stats.AbilityPower.Total * 0.3f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + shieldAp;

            // 对范围内敌人造成伤害
            foreach (var unit in GetUnitsInRange(owner.Position, 200f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 添加护盾
            AddBuff("Shield", 1.5f, 1, spell, owner, owner);

            AddParticle(owner, owner, "PaleCascade_cas", owner.Position);
        }
    }
}
