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
    /// Varus Q - Piercing Arrow
    /// 蓄力后发射一支穿透箭，对路径上的所有敌人造成物理伤害
    /// 每命中一个敌人伤害降低15%，最低33%
    /// </summary>
    public class VarusQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            MissileParameters = new MissileParameters { Type = MissileType.Ballistic }
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 最小伤害: 10/43/77/110/143 (+1.0 AD)
            // 最大伤害: 15/65/115/165/215 (+1.6 AD)
            float[] minDamage = { 10f, 43f, 77f, 110f, 143f };
            float[] maxDamage = { 15f, 65f, 115f, 165f, 215f };
            float ad = owner.Stats.AttackDamage.Total;
            
            float minDmg = minDamage[spell.CastInfo.SpellLevel - 1] + ad;
            float maxDmg = maxDamage[spell.CastInfo.SpellLevel - 1] + ad * 1.6f;
            
            // 简化处理，使用平均伤害
            float damage = (minDmg + maxDmg) / 2f;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对路径上的敌人造成伤害
            foreach (var unit in GetUnitsInRange(targetPos, 500f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }
        }
    }
}
