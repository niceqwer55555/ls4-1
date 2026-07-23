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
    /// Veigar Q - Baleful Strike
    /// 邪恶冲击
    /// </summary>
    public class VeigarQ : ISpellScript
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

            // 魔法伤害: 70/110/150/190/230 (+0.6 AP)
            float[] baseDamage = { 70f, 110f, 150f, 190f, 230f };
            float ap = owner.Stats.AbilityPower.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap * 0.6f;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            AddParticle(owner, target, "VeigarQ_cas", target.Position);
        }
    }
}
