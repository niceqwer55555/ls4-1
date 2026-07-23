using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using System.Numerics;

namespace Spells
{
    /// <summary>
    /// LeBlanc Q - Sigil of Malice
    /// Damage: 55/80/105/130/155 (+40% AP)
    /// Mimic damage: same values (+40% AP)
    /// </summary>
    public class LeblancChaosOrb : ISpellScript
    {
        bool IsCrit;
        float Damage;
        float BaseDamage;
        float ROrbDamage;
        ObjAIBase Leblanc;
        SpellMissile Missile;
        public SpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            Leblanc = owner = spell.CastInfo.Owner as Champion;
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnSpellPostCast(Spell spell)
        {
            Missile = spell.CreateSpellMissile(new MissileParameters { Type = MissileType.Target });
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            Missile = missile;
            // Q damage: 55/80/105/130/155 (+40% AP)
            float[] qBase = { 55f, 80f, 105f, 130f, 155f };
            BaseDamage = qBase[Leblanc.Spells[0].CastInfo.SpellLevel - 1] + (Leblanc.Stats.AbilityPower.Total * 0.4f);
            // RQ damage: same base (+40% AP)
            ROrbDamage = BaseDamage;

            if (target.HasBuff("LeblancChaosOrb")) { IsCrit = true; Damage = BaseDamage * 2; target.RemoveBuffsWithName("LeblancChaosOrb"); }
            else if (target.HasBuff("LeblancChaosOrbM")) { IsCrit = true; Damage = ROrbDamage * 2; target.RemoveBuffsWithName("LeblancChaosOrbM"); }
            else { IsCrit = false; Damage = BaseDamage; }

            target.TakeDamage(Leblanc, Damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, IsCrit);
            AddParticleTarget(Leblanc, target, "LeBlanc_Base_Q_tar", target);
            AddBuff("LeblancChaosOrb", 3.5f, 1, spell, target, Leblanc);
            Missile.SetToRemove();
        }
    }

    /// <summary>
    /// LeBlanc RQ - Mimic Sigil of Malice
    /// Damage: 55/80/105/130/155 (+40% AP), proc deals same again
    /// </summary>
    public class LeblancChaosOrbM : ISpellScript
    {
        bool Crit;
        float Damage;
        Spell RQSpell;
        float QOrbDamage;
        float BaseDamage;
        ObjAIBase Leblanc;
        SpellMissile Missile;
        AttackableUnit Target;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            RQSpell = spell;
            Leblanc = owner = spell.CastInfo.Owner as Champion;
        }

        public void OnSpellPostCast(Spell spell)
        {
            Missile = spell.CreateSpellMissile(new MissileParameters { Type = MissileType.Target });
            ApiEventManager.OnSpellMissileHit.AddListener(this, Missile, TargetExecute, false);
            // RQ mimics Q damage: 55/80/105/130/155 (+40% AP)
            float[] qBase = { 55f, 80f, 105f, 130f, 155f };
            QOrbDamage = qBase[Leblanc.Spells[0].CastInfo.SpellLevel - 1] + (Leblanc.Stats.AbilityPower.Total * 0.4f);
            BaseDamage = QOrbDamage;
        }

        public void TargetExecute(SpellMissile missile, AttackableUnit target)
        {
            if (target.HasBuff("LeblancChaosOrb")) { target.RemoveBuffsWithName("LeblancChaosOrb"); Crit = true; Damage = BaseDamage + QOrbDamage; }
            else if (target.HasBuff("LeblancChaosOrbM")) { target.RemoveBuffsWithName("LeblancChaosOrbM"); Crit = true; Damage = BaseDamage * 2; }
            else { Crit = false; Damage = BaseDamage; }

            target.TakeDamage(Leblanc, Damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, Crit);
            AddParticleTarget(Leblanc, target, "LeBlanc_Base_RQ_tar", target);
            AddBuff("LeblancChaosOrbM", 3.5f, 1, RQSpell, target, Leblanc);
            missile.SetToRemove();
        }
    }
}
