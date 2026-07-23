using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System;
using System.Numerics;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    /// <summary>
    /// Gangplank W - Remove Scurvy
    /// Heals and removes all crowd control.
    /// Heal: 50/75/100/125/150 (+90% AP) + 15% missing health
    /// </summary>
    public class RemoveScurvy : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            owner.StopMovement();

            // Heal: 50/75/100/125/150 (+90% AP) + 15% missing health
            float[] baseHeal = { 50f, 75f, 100f, 125f, 150f };
            float ap = owner.Stats.AbilityPower.Total * 0.9f;
            float missingHealth = owner.Stats.HealthPoints.Total - owner.Stats.CurrentHealth;
            float missingHealthHeal = missingHealth * 0.15f;
            float totalHeal = baseHeal[spell.CastInfo.SpellLevel - 1] + ap + missingHealthHeal;

            owner.Stats.CurrentHealth = Math.Min(owner.Stats.CurrentHealth + totalHeal, owner.Stats.HealthPoints.Total);

            // Remove all crowd control
            owner.SetStatus(StatusFlags.CanAttack, true);
            owner.SetStatus(StatusFlags.CanCast, true);
            owner.SetStatus(StatusFlags.CanMove, true);
            owner.SetStatus(StatusFlags.Stunned, false);
            owner.SetStatus(StatusFlags.Rooted, false);
            owner.SetStatus(StatusFlags.Disarmed, false);
            owner.SetStatus(StatusFlags.Suppressed, false);
            owner.SetStatus(StatusFlags.Silenced, false);
            owner.SetStatus(StatusFlags.Charmed, false);
            owner.SetStatus(StatusFlags.Feared, false);
            owner.SetStatus(StatusFlags.Taunted, false);
            owner.SetStatus(StatusFlags.Sleep, false);
            owner.SetStatus(StatusFlags.Netted, false);

            owner.Stats.SetActionState(ActionState.CAN_ATTACK, true);
            owner.Stats.SetActionState(ActionState.CAN_CAST, true);
            owner.Stats.SetActionState(ActionState.CAN_MOVE, true);

            // Add particle effect
            AddParticleTarget(owner, owner, "pirate_cannonBarrage_tar.troy", owner);
        }
    }
}
