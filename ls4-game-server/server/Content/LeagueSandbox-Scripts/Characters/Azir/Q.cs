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
    /// Azir Q - Conquering Sands
    /// 指挥沙兵攻击敌人
    /// </summary>
    public class AzirQ : ISpellScript
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

            // 魔法伤害: 70/90/110/130/150 (+0.5 AP)
            float[] baseDamage = { 70f, 90f, 110f, 130f, 150f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            foreach (var unit in GetUnitsInRange(targetPos, 200f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticle(owner, owner, "AzirQ_cas", targetPos);
        }
    }
}
