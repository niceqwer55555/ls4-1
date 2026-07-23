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
    /// Jhin Q - Dancing Grenades
    /// 投掷弹跳手雷伤害敌人
    /// </summary>
    public class JhinQ : ISpellScript
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

            // 物理伤害: 80/115/150/185/220 (+0.8 AD)
            float[] baseDamage = { 80f, 115f, 150f, 185f, 220f };
            float ad = owner.Stats.AttackDamage.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad * 0.8f;

            foreach (var unit in GetUnitsInRange(targetPos, 200f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticle(owner, owner, "JhinQ_cas", targetPos);
        }
    }
}
