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
    /// Quinn Q - Blinding Assault
    /// 投掷羽刃造成物理伤害并致盲
    /// </summary>
    public class QuinnQ : ISpellScript
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

            // 物理伤害: 20/45/70/95/120 (+0.35 AD) (+0.5 AP)
            float[] baseDamage = { 20f, 45f, 70f, 95f, 120f };
            float ad = owner.Stats.AttackDamage.Total;
            float ap = owner.Stats.AbilityPower.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad * 0.35f + ap * 0.5f;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 致盲: 1.25/1.5/1.75/2/2.25秒
            float[] duration = { 1.25f, 1.5f, 1.75f, 2f, 2.25f };
            AddBuff("QuinnQ", duration[spell.CastInfo.SpellLevel - 1], 1, spell, target, owner);

            AddParticle(owner, owner, "QuinnQ_cas", target.Position);
        }
    }
}
