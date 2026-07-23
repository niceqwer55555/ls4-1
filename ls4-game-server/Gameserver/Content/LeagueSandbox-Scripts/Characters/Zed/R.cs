using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using LeagueSandbox.GameServer.GameObjects;

namespace Spells
{
    /// <summary>
    /// Zed R - Death Mark
    /// Becomes untargetable, dashes to target, marks them for 3 seconds
    /// After 3 seconds, the mark detonates dealing 20/35/50% of all damage dealt during the mark
    /// </summary>
    public class ZedUlt : ISpellScript
    {
        private Spell Ult;
        private ObjAIBase Zed;
        public static AttackableUnit Target;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Ult = spell;
            Target = target;
            Zed = owner = Ult.CastInfo.Owner as Champion;
            PlayAnimation(Zed, "Spell4");
            Zed.SetTargetUnit(null, true);
            Zed.CancelAutoAttack(false, true);
        }

        public void OnSpellCast(Spell spell)
        {
            // Buffs for the death mark duration
            AddBuff("ZedUltBuff", 1.5f, 1, Ult, Zed, Zed);
            AddBuff("ZedR2", 5.9f, 1, Ult, Zed, Zed);
            AddBuff("ZedRHandler", 6.0f, 1, Ult, Zed, Zed, false);
            AddBuff("ZedUlt", 0.7f, 1, Ult, Zed, Zed);
            AddParticleTarget(Zed, Target, "Zed_Base_R_tar_TargetMarker.troy", Target, 10f);

            if (Target != null)
            {
                TeleportTo(Zed, Target.Position.X, Target.Position.Y);
                // Initial damage: 50/125/200 (+100% bonus AD)
                float[] baseDamage = { 50f, 125f, 200f };
                float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + Zed.Stats.AttackDamage.FlatBonus * 1.0f;
                Target.TakeDamage(Zed, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            }
        }

        public void OnSpellPostCast(Spell spell)
        {
            Ult.SetCooldown(0.5f, true);
        }
    }

    /// <summary>
    /// Zed R2 - re-cast to swap with shadow
    /// </summary>
    public class ZedR2 : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };
    }
}
