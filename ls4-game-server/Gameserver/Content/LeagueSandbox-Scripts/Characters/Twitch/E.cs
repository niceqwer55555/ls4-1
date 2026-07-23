using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects;

namespace Spells
{
    /// <summary>
    /// Twitch E - Expunge
    /// Deals physical damage to all nearby enemies affected by Deadly Venom
    /// Base: 15/20/25/30/35 (+20% AP)
    /// Per stack: 20/35/50/65/80 (+20% AP)
    /// </summary>
    public class TwitchExpunge : ISpellScript
    {
        Buff Venom;
        ObjAIBase Twitch;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            Twitch = owner = spell.CastInfo.Owner as Champion;
            ApiEventManager.OnLevelUpSpell.AddListener(this, spell, OnLevelUp, true);
        }

        public void OnLevelUp(Spell spell)
        {
            AddBuff("TwitchExpunge", 250000f, 1, spell, Twitch, Twitch);
        }

        public void OnSpellPostCast(Spell spell)
        {
            foreach (AttackableUnit Unit in GetUnitsInRange(Twitch.Position, 1100f, true))
            {
                Venom = Unit.GetBuffWithName("TwitchDeadlyVenom");
                if (Venom != null && Venom.SourceUnit == Twitch)
                {
                    SpellCast(Twitch, 2, SpellSlotType.ExtraSlots, false, Unit, Vector2.Zero);
                }
            }
        }
    }

    /// <summary>
    /// Twitch E Missile - deals stack-based damage
    /// </summary>
    public class TwitchEParticle : ISpellScript
    {
        ObjAIBase Twitch;
        SpellMissile Missile;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            Twitch = spell.CastInfo.Owner as Champion;
            Missile = spell.CreateSpellMissile(new MissileParameters
            {
                Type = MissileType.Target,
            });
            ApiEventManager.OnSpellMissileHit.AddListener(this, Missile, TargetExecute, false);
        }

        public void TargetExecute(SpellMissile missile, AttackableUnit target)
        {
            // Base damage: 15/20/25/30/35 (+20% AP)
            float[] baseDamage = { 15f, 20f, 25f, 30f, 35f };
            // Bonus per stack: 20/35/50/65/80 (+20% AP)
            float[] stackDamage = { 20f, 35f, 50f, 65f, 80f };
            var spellLevel = Twitch.Spells[2].CastInfo.SpellLevel;

            float damage = baseDamage[spellLevel - 1] + stackDamage[spellLevel - 1];

            // Add AP ratio
            damage += (float)(Twitch.Stats.AbilityPower.Total * 0.2);

            // Multiply by number of venom stacks
            var venomBuff = target.GetBuffWithName("TwitchDeadlyVenom");
            int stacks = venomBuff != null ? venomBuff.StackCount : 1;
            damage *= stacks;

            target.TakeDamage(Twitch, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddParticleTarget(Twitch, target, "Twitch_Base_E_Tar.troy", target);

            // Remove venom stacks after expunge
            if (venomBuff != null)
            {
                venomBuff.DeactivateBuff();
            }
        }
    }
}
