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
    /// Velkoz Q - Plasma Fission
    /// 发射分裂等离子体伤害并减速敌人
    /// </summary>
    public class VelkozQ : ISpellScript
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

            // 魔法伤害: 40/60/80/100/120 (+0.4 AP)
            float[] baseDamage = { 40f, 60f, 80f, 100f, 120f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 减速: 30/35/40/45/50%
            float[] slowAmount = { 0.30f, 0.35f, 0.40f, 0.45f, 0.50f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];
            AddBuff("Slow", 2f, 1, spell, target, owner);

            AddParticle(owner, owner, "VelkozQ_cas", target.Position);
        }
    }
}
