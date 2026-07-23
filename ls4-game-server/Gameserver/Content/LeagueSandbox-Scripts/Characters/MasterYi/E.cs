using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using System.Numerics;

namespace Spells
{
    /// <summary>
    /// MasterYi E - Wuju Style
    /// Passive: +10% AD. Active: attacks deal bonus true damage for 5s,
    /// and passive AD bonus becomes 10/12.5/15/17.5/20%.
    /// </summary>
    public class WujuStyle : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            NotSingleTargetSpell = true,
            IsDamagingSpell = true
        };

        private ObjAIBase _owner;
        private Spell _spell;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            _owner = owner;
            _spell = spell;
            // Apply passive buff (+10% AD) when E is learned
            AddBuff("WujuStylePassive", 1f, 1, spell, owner, owner, true);
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
            // Remove passive buff when E is unlearned
            RemoveBuff(owner, "WujuStylePassive");
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var spellLevel = spell.CastInfo.SpellLevel;

            // Remove passive buff (will be replaced by active buff which has higher AD%)
            RemoveBuff(owner, "WujuStylePassive");

            // Add active buff (5 seconds) - bonus true damage on hit + enhanced AD%
            AddBuff("WujuStyleSuperChargedVisual", 5.0f, 1, spell, owner, owner);

            // Add visual particle
            AddParticleTarget(owner, owner, "MasterYi_Base_E_Activate.troy", owner, 5.0f);
        }

        public void OnSpellPostCast(Spell spell)
        {
        }

        public void OnSpellChannel(Spell spell)
        {
        }

        public void OnSpellChannelCancel(Spell spell, ChannelingStopSource reason)
        {
        }

        public void OnSpellPostChannel(Spell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
