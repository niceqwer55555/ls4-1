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
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    /// <summary>
    /// Talon Q - Noxian Diplomacy
    /// Empowers next auto to deal 30/60/90/120/150 (+130% bonus AD) bonus physical damage
    /// Applies bleed if target is champion
    /// </summary>
    public class TalonNoxianDiplomacy : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            owner.CancelAutoAttack(true);
            AddBuff("TalonNoxianDiplomacyBuff", 6.0f, 1, spell, owner, owner);
        }

        public void OnSpellPostCast(Spell spell)
        {
            spell.SetCooldown(0, true);
        }
    }

    /// <summary>
    /// Talon Q Auto Attack modifier
    /// Damage: 30/60/90/120/150 (+130% bonus AD)
    /// </summary>
    public class TalonNoxianDiplomacyAttack : ISpellScript
    {
        float QDamage;
        ObjAIBase Talon;
        AttackableUnit Target;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Target = target;
            Talon = owner = spell.CastInfo.Owner as Champion;
        }

        public void OnSpellCast(Spell spell)
        {
            AddParticleTarget(Talon, Target, "Talon_Base_Q_Bleed", Target, 10f, 1f);
            if (Target is Champion) { AddBuff("TalonBleedDebuff", 6f, 1, spell, Target, Talon); }

            // Damage: 30/60/90/120/150 (+130% bonus AD) from TalonNoxianDiplomacy Effect2
            float[] baseDamage = { 30f, 60f, 90f, 120f, 150f };
            float bonusAD = Talon.Stats.AttackDamage.FlatBonus;
            QDamage = baseDamage[Talon.Spells[0].CastInfo.SpellLevel - 1] + (bonusAD * 1.3f);

            // Damage amp from E (Cutthroat)
            if (Talon.HasBuff("TalonDamageAmp"))
            {
                QDamage *= 1f + (0.03f * Talon.Spells[2].CastInfo.SpellLevel);
            }

            if (Talon.IsNextAutoCrit)
            {
                Target.TakeDamage(Talon, QDamage * 2, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, true);
            }
            else
            {
                Target.TakeDamage(Talon, QDamage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            }
        }
    }
}
