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
    /// Udyr R - Phoenix Stance
    /// 获得法术强度和火焰光环
    /// </summary>
    public class UdyrR : ISpellScript
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

            // 每秒魔法伤害: 40/80/120/160/200 (+0.45 AP)
            float[] dpsDamage = { 40f, 80f, 120f, 160f, 200f };
            float ap = owner.Stats.AbilityPower.Total * 0.45f;
            float dps = dpsDamage[spell.CastInfo.SpellLevel - 1] + ap;

            foreach (var unit in GetUnitsInRange(owner.Position, 300f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, dps, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            // 持续时间: 5秒
            AddBuff("UdyrR", 5f, 1, spell, owner, owner);
        }
    }
}
