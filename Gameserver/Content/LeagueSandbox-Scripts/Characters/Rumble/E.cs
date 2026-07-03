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
    /// Rumble E - Toxic Shot
    /// 发射毒液火箭，减速敌人
    /// </summary>
    public class RumbleE : ISpellScript
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

            // 魔法伤害: 50/75/100/125/150 (+0.4 AP)
            float[] baseDamage = { 50f, 75f, 100f, 125f, 150f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 减速: 35/40/45/50/55%
            float[] slowAmount = { 0.35f, 0.40f, 0.45f, 0.50f, 0.55f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];
            AddBuff("Slow", 3f, 1, spell, target, owner);

            AddParticle(owner, owner, "RumbleE_cas", target.Position);
        }
    }
}
