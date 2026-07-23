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
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings;
using System.Collections.Generic;

namespace Spells
{
    /// <summary>
    /// Zed Q - Razor Shuriken
    /// Throws a shuriken dealing 75/110/145/180/215 (+100% bonus AD) physical damage
    /// Each additional target hit takes 50% less damage
    /// </summary>
    public class ZedShuriken : ISpellScript
    {
        private Spell Shuriken;
        private ObjAIBase Zed;
        private Vector2 TargetPos;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            Shuriken = spell;
            Zed = owner = spell.CastInfo.Owner as Champion;
        }

        public void OnSpellCast(Spell spell)
        {
            AddBuff("ZedShuriken", 0.5f, 1, spell, Zed, Zed, false);
        }

        public void OnSpellPostCast(Spell spell)
        {
            TargetPos = GetPointFromUnit(Zed, 950f);
            FaceDirection(TargetPos, Zed);
            SpellCast(Zed, 1, SpellSlotType.ExtraSlots, TargetPos, TargetPos, true, Vector2.Zero);
        }
    }

    /// <summary>
    /// Zed Q Missile
    /// </summary>
    public class ZedShurikenMisOne : ISpellScript
    {
        float Damage;
        private Spell Shuriken;
        private ObjAIBase Zed;
        private SpellMissile Missile;
        public List<AttackableUnit> UnitsHit = new List<AttackableUnit>();
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            IsDamagingSpell = true,
            TriggersSpellCasts = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            UnitsHit.Clear();
            Zed = owner = spell.CastInfo.Owner as Champion;
            Missile = spell.CreateSpellMissile(new MissileParameters { Type = MissileType.Circle, OverrideEndPosition = end });
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            if (!UnitsHit.Contains(target))
            {
                UnitsHit.Add(target);
                // Damage: 75/110/145/180/215 (+100% bonus AD)
                float[] baseDamage = { 75f, 110f, 145f, 180f, 215f };
                Damage = baseDamage[Zed.Spells[0].CastInfo.SpellLevel - 1] + Zed.Stats.AttackDamage.FlatBonus * 1.0f;

                // Each additional target hit after the first takes 50% less damage
                if (UnitsHit.Count > 1)
                {
                    Damage *= 0.5f;
                }

                target.TakeDamage(Zed, Damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                AddParticleTarget(Zed, target, "Zed_Base_Q_tar", target, 1f);
            }
        }
    }

    public class ZedRakeMissileTwo : ZedShurikenMisOne
    {
    }

    public class ZedRakeMissileThree : ZedShurikenMisOne
    {
    }
}
