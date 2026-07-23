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
    /// Corki E - Gatling Gun
    /// 扫射造成物理伤害并减少护甲
    /// </summary>
    public class CorkiE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 物理伤害: 20/35/50/65/80 (+0.2 AD)
            float[] baseDamage = { 20f, 35f, 50f, 65f, 80f };
            float ad = owner.Stats.AttackDamage.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad * 0.2f;

            // 护甲减少: 2/3/4/5/6 每层
            float[] armorReduction = { 2f, 3f, 4f, 5f, 6f };
            float reduction = armorReduction[spell.CastInfo.SpellLevel - 1];

            // 对前方锥形区域敌人造成伤害
            foreach (var unit in GetUnitsInRange(owner.Position, 600f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticle(owner, owner, "CorkiE_cas", owner.Position);
        }
    }
}
