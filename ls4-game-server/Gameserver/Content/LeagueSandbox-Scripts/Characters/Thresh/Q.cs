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
    /// Thresh Q - Death Sentence
    /// 死亡判决
    /// </summary>
    public class ThreshQ : ISpellScript
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

            float[] baseDamage = { 80f, 120f, 160f, 200f, 240f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            AddBuff("Stun", 1.5f, 1, spell, target, owner);
            AddBuff("ThreshQHooked", 2f, 1, spell, target, owner);

            AddParticle(owner, owner, "ThreshQ_cas", target.Position);
        }
    }

    public class ThreshQ2 : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            foreach (var unit in GetUnitsInRange(owner.Position, 1000f, true))
            {
                var buff = unit.GetBuffWithName("ThreshQHooked");
                if (buff != null && buff.SourceUnit == owner)
                {
                    RemoveBuff(buff);
                    TeleportTo(owner, unit.Position.X, unit.Position.Y);
                    break;
                }
            }
        }
    }
}
