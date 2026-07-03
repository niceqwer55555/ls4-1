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
    /// Veigar E - Event Horizon
    /// 事件视界
    /// </summary>
    public class VeigarE : ISpellScript
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

            var target = spell.CastInfo.Targets[0].Unit;

            // 魔法伤害: 60/90/120/150/180 (+0.4 AP)
            float[] baseDamage = { 60f, 90f, 120f, 150f, 180f };
            float ap = owner.Stats.AbilityPower.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap * 0.4f;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 眩晕
            AddBuff("Stun", 2.5f, 1, spell, target, owner);

            AddParticle(owner, target, "VeigarE_cas", target.Position);
        }
    }
}
