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
    /// Zed E - Shadow Slash
    /// Slashes nearby enemies dealing 30/50/70/90/110 (+80% bonus AD) physical damage
    /// Slows by 20/25/30/35/40% for 1.5 seconds
    /// Reduces W cooldown by 2 seconds per champion hit
    /// </summary>
    public class ZedPBAOEDummy : ISpellScript
    {
        private ObjAIBase Zed;
        public List<AttackableUnit> UnitsHit = Spells.ZedPBAOE.UnitsHit;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            UnitsHit.Clear();
            Zed = owner = spell.CastInfo.Owner as Champion;
        }

        public void OnSpellCast(Spell spell)
        {
            PlayAnimation(Zed, "Spell3", 0.5f);
            AddParticleTarget(Zed, null, "Zed_Base_E_cas.troy", Zed);
            AddBuff("ZedPBAOEDummy", 0.5f, 1, spell, Zed, Zed, false);
            SpellCast(Zed, 2, SpellSlotType.ExtraSlots, true, Zed, Vector2.Zero);
        }
    }

    /// <summary>
    /// Zed E - damage and slow on hit
    /// </summary>
    public class ZedPBAOE : ISpellScript
    {
        float Damage;
        private Spell AOE;
        private ObjAIBase Zed;
        public static List<AttackableUnit> UnitsHit = new List<AttackableUnit>();
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            AOE = spell;
            Zed = owner = spell.CastInfo.Owner as Champion;
            ApiEventManager.OnSpellHit.AddListener(this, AOE, TargetExecute, false);
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            AOE.CreateSpellSector(new SectorParameters { Length = 250f, SingleTick = true, Type = SectorType.Area });
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            if (Zed != target && !UnitsHit.Contains(target))
            {
                UnitsHit.Add(target);
                // Reduce W cooldown by 2 per champion hit
                Zed.Spells[1].LowerCooldown(2);
                // Slow: 20/25/30/35/40% for 1.5s
                AddBuff("ZedSlow", 1.5f, 1, spell, target, Zed);
                AddParticleTarget(Zed, target, "Zed_Base_E_tar", target, 1f);
                // Damage: 30/50/70/90/110 (+80% bonus AD)
                float[] baseDamage = { 30f, 50f, 70f, 90f, 110f };
                Damage = baseDamage[Zed.Spells[2].CastInfo.SpellLevel - 1] + Zed.Stats.AttackDamage.FlatBonus * 0.8f;
                target.TakeDamage(Zed, Damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            }
        }
    }
}
