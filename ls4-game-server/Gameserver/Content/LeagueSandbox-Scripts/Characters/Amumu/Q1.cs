using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using System.Numerics;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    /// <summary>
    /// Amumu Q - Bandage Toss Missile
    /// Damage: 80/140/200/260/320 (+70% AP), stuns for 1s and pulls Amumu to target
    /// </summary>
    public class SadMummyBandageToss : ISpellScript
    {
        ObjAIBase Owner;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true
        };
        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }
        public void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            // Damage: 80/140/200/260/320 (+70% AP)
            float[] baseDamage = { 80f, 140f, 200f, 260f, 320f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + owner.Stats.AbilityPower.Total * 0.7f;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddParticleTarget(owner, target, "BandageToss_mis.troy", target, 2f, 1f);
            // Stun for 1 second
            AddBuff("Stun", 1.0f, 1, spell, target, owner);
            // Pull Amumu to target
            ForceMovement(owner, "Spell1", target.Position, 2200, 0, 0, 0);
            missile.SetToRemove();
        }
    }
}
