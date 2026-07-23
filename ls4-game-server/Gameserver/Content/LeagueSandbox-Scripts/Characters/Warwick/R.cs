using System.Numerics;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;
using System;

namespace Spells
{
    /// <summary>
    /// Warwick R - Infinite Duress
    /// Leaps to target, suppresses for 1.8s, deals 250/335/420 (+167% AP) magic damage
    /// Also applies on-hit effects 3 times
    /// </summary>
    public class InfiniteDuress : ISpellScript
    {
        AttackableUnit Target;
        private ObjAIBase Warwick;

        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Target = target;
            Warwick = owner;
        }

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;

            var target = spell.CastInfo.Targets[0].Unit;
            var current = new Vector2(owner.Position.X, owner.Position.Y);
            var to = Vector2.Normalize(new Vector2(target.Position.X, target.Position.Y) - current);

            // Suppress both Warwick and target
            AddBuff("InfiniteDuress", 1.8f, 1, spell, owner, owner, false);
            AddBuff("Suppression", 1.8f, 1, spell, target, owner, false);

            PlayAnimation(owner, "Spell4_loop", 0.7f);
            ForceMovement(owner, "Spell4", target.Position, 2200, 0, 0, 0);

            AddParticleTarget(owner, Target, "InfiniteDuress_tar.troy", Target, 1f, 1f);

            // Deal damage in 5 ticks over 1.8 seconds
            // Total: 250/335/420 (+167% AP)
            float[] baseDamage = { 250f, 335f, 420f };
            float totalDamage = baseDamage[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 1.67f;
            float damagePerTick = totalDamage / 5f;

            for (int i = 0; i < 5; i++)
            {
                var delay = 0.36f * (i + 1);
                CreateTimer(delay, () =>
                {
                    if (target != null && !target.IsDead)
                    {
                        target.TakeDamage(owner, damagePerTick, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    }
                });
            }

            CreateTimer(1.8f, () =>
            {
                if (owner != null && !owner.IsDead)
                {
                    PlayAnimation(owner, "Spell4_Winddown", 0.7f);
                }
            });
        }
    }
}
