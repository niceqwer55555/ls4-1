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
    /// Nocturne W - Unspeakable Horror
    /// 对目标造成持续魔法伤害和恐惧
    /// </summary>
    public class NocturneW : ISpellScript
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

            // 每秒魔法伤害: 15/20/25/30/35 (+0.6 AP)
            float[] dpsDamage = { 15f, 20f, 25f, 30f, 35f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float dps = dpsDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, dps, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 恐惧持续: 1.25/1.5/1.75/2/2.25秒
            float[] fearDuration = { 1.25f, 1.5f, 1.75f, 2f, 2.25f };
            AddBuff("Fear", fearDuration[spell.CastInfo.SpellLevel - 1], 1, spell, target, owner);
        }
    }
}
