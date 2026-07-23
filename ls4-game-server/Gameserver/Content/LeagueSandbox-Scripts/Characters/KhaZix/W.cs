using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings;
using System.Numerics;

namespace Spells
{
    /// <summary>
    /// KhaZix W - Void Spike
    /// Fires spikes dealing 80/110/140/170/200 (+100% bonus AD) physical damage
    /// Also slows by 20% for 2 seconds
    /// </summary>
    public class KhazixW : ISpellScript
    {
        ObjAIBase Khazix;
        Vector2 TargetPos;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            IsDamagingSpell = true,
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            Khazix = spell.CastInfo.Owner as Champion;
            TargetPos = GetPointFromUnit(Khazix, 1000.0f);
            FaceDirection(TargetPos, Khazix);
            SpellCast(Khazix, 0, SpellSlotType.ExtraSlots, TargetPos, TargetPos, true, Vector2.Zero);
        }
    }

    /// <summary>
    /// KhaZix W Missile
    /// </summary>
    public class KhazixWMissile : ISpellScript
    {
        float Damage;
        ObjAIBase Khazix;
        Vector2 TargetPos;
        SpellMissile Missile;
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
            Khazix = spell.CastInfo.Owner as Champion;
            Missile = spell.CreateSpellMissile(new MissileParameters { Type = MissileType.Circle, OverrideEndPosition = end });
        }

        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            // Damage: 80/110/140/170/200 (+100% bonus AD)
            float[] baseDamage = { 80f, 110f, 140f, 170f, 200f };
            Damage = baseDamage[spell.CastInfo.SpellLevel - 1] + Khazix.Stats.AttackDamage.FlatBonus * 1.0f;

            if (target.Team != Khazix.Team && !(target is ObjBuilding || target is BaseTurret))
            {
                target.TakeDamage(Khazix, Damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                AddBuff("Slow", 2f, 1, spell, target, Khazix, false);
                AddParticleTarget(Khazix, target, "Khazix_Base_W_Tar", target);
            }
            missile.SetToRemove();
        }
    }
}
