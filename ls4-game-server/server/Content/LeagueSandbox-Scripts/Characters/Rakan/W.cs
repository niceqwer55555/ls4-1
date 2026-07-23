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
    /// Rakan W - Grand Plaza
    /// 盛大广场
    /// </summary>
    public class RakanW : ISpellScript
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

            // 魔法伤害: 70/115/160/205/250 (+0.7 AP)
            float[] baseDamage = { 70f, 115f, 160f, 205f, 250f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 位移到目标位置
            TeleportTo(owner, targetPos.X, targetPos.Y);

            foreach (var unit in GetUnitsInRange(targetPos, 300f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    // 击飞
                    AddBuff("Knockup", 1f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "RakanW_cas", targetPos);
        }
    }
}
