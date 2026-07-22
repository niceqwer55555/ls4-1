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
    /// Sejuani E - Permafrost
    /// 冻结并减速被冰霜击中过的敌人
    /// </summary>
    public class SejuaniE : ISpellScript
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
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 减速: 50/55/60/65/70%
            float[] slowAmount = { 0.50f, 0.55f, 0.60f, 0.65f, 0.70f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];
            AddBuff("SejuaniE", 2f, 1, spell, target, owner);

            AddParticle(owner, owner, "SejuaniE_cas", target.Position);
        }
    }
}
