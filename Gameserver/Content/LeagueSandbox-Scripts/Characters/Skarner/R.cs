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
    /// Skarner R - Impale
    /// 刺穿敌人造成伤害并眩晕
    /// </summary>
    public class SkarnerR : ISpellScript
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
            var targetPos = target.Position;

            // 物理伤害: 100/150/200 (+0.5 AD)
            float[] baseDamage = { 100f, 150f, 200f };
            float ad = owner.Stats.AttackDamage.Total;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad * 0.5f;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 眩晕: 1.5/1.75/2秒
            float[] duration = { 1.5f, 1.75f, 2f };
            AddBuff("Stun", duration[spell.CastInfo.SpellLevel - 1], 1, spell, target, owner);

            AddParticle(owner, owner, "SkarnerR_cas", targetPos);
        }
    }
}
