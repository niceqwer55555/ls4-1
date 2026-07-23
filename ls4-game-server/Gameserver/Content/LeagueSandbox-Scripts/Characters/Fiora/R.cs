using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects;
using System.Numerics;
using System.Collections.Generic;

namespace Spells
{
    /// <summary>
    /// Fiora R - Blade Waltz
    /// Attacks target 5 times over 2.25 seconds
    /// Each strike deals 130/150/170 (+120% bonus AD) physical damage
    /// Repeated hits on same target deal 25% damage
    /// </summary>
    public class FioraDance : ISpellScript
    {
        ObjAIBase Fiora;
        Vector2 TargetPos;
        AttackableUnit Target;
        public List<AttackableUnit> UnitsHit = Spells.FioraDanceStrike.UnitsHit;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            UnitsHit.Clear();
            Target = spell.CastInfo.Targets[0].Unit;
            Fiora = spell.CastInfo.Owner as Champion;
            Fiora.SetTargetUnit(null, true);
            AddBuff("FioraDance", 2.25f, 1, spell, Fiora, Fiora);
        }
    }

    /// <summary>
    /// Fiora R Strike
    /// </summary>
    public class FioraDanceStrike : ISpellScript
    {
        float Damage;
        Spell Dance;
        Particle Trail;
        Buff DanceBuff;
        ObjAIBase Fiora;
        Vector2 TargetPos;
        AttackableUnit Target;
        public static List<AttackableUnit> UnitsHit = new List<AttackableUnit>();
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Dance = spell;
            Target = target;
            Fiora = owner = spell.CastInfo.Owner as Champion;
            AddParticleTarget(Fiora, Fiora, "Fiora_Dance_cas", Fiora);
            AddParticleTarget(Fiora, Fiora, "Fiora_Dance_windup", Fiora);
            ForceMovement(Fiora, null, new Vector2(Fiora.Position.X + 40f, Fiora.Position.Y + 40f), 110f, 0, 150f, 0);
            TargetPos = GetPointFromUnit(Fiora, System.Math.Abs(Vector2.Distance(Target.Position, Fiora.Position)) + 175);
        }

        public void OnSpellPostCast(Spell spell)
        {
            Fiora.SetDashingState(false);
            PlayAnimation(Fiora, "spell4c", 0.3f);
            TeleportTo(Fiora, TargetPos.X, TargetPos.Y);
            AddParticleTarget(Fiora, Target, "Fiora_Dance_tar", Target);

            // Damage: 130/150/170 (+120% bonus AD)
            float[] baseDamage = { 130f, 150f, 170f };
            float fullDamage = baseDamage[Fiora.Spells[3].CastInfo.SpellLevel - 1] + Fiora.Stats.AttackDamage.FlatBonus * 1.2f;

            if (!UnitsHit.Contains(Target))
            {
                UnitsHit.Add(Target);
                Damage = fullDamage;
            }
            else
            {
                // Repeated hits deal 25% damage
                Damage = fullDamage * 0.25f;
            }
            Target.TakeDamage(Fiora, Damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }
    }
}
