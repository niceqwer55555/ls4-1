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
    /// Malzahar E - Malefic Vison
    /// 释放虚空幻象沉默敌人
    /// </summary>
    public class MalzaharE : ISpellScript
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

            // 魔法伤害: 80/115/150/185/220 (+0.55 AP)
            float[] baseDamage = { 80f, 115f, 150f, 185f, 220f };
            float ap = owner.Stats.AbilityPower.Total * 0.55f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 沉默
            AddBuff("Silence", 2f, 1, spell, target, owner);

            AddParticle(owner, owner, "MalzaharE_cas", target.Position);
        }
    }
}
