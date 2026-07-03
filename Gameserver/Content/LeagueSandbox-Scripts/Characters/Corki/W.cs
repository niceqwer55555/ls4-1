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
    /// Corki W - Valkyrie
    /// 俯冲并留下伤害路径
    /// </summary>
    public class CorkiW : ISpellScript
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

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 突进到目标位置
            TeleportTo(owner, targetPos.X, targetPos.Y);

            // 每秒魔法伤害: 60/90/120/150/180 (+0.4 AP)
            float[] dpsDamage = { 60f, 90f, 120f, 150f, 180f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float dps = dpsDamage[spell.CastInfo.SpellLevel - 1] + ap;

            foreach (var unit in GetUnitsInRange(targetPos, 200f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, dps, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticle(owner, owner, "CorkiW_cas", targetPos);
        }
    }
}
