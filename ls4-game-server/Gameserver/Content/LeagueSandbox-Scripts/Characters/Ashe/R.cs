using System;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    /// <summary>
    /// Ashe R - Enchanted Crystal Arrow
    /// Damage: 250/425/600 (+100% AP), stuns champions for up to 3.5s based on distance
    /// </summary>
    public class EnchantedCrystalArrow : ISpellScript
    {
        Spell Arrow;
        ObjAIBase Ashe;
        SpellMissile Missile;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            Arrow = spell;
            Ashe = spell.CastInfo.Owner as Champion;
            Missile = spell.CreateSpellMissile(new MissileParameters { Type = MissileType.Circle });
            ApiEventManager.OnSpellMissileHit.AddListener(this, Missile, TargetExecute, false);
        }

        public void TargetExecute(SpellMissile missile, AttackableUnit target)
        {
            // Damage: 250/425/600 (+100% AP)
            float[] baseDamage = { 250f, 425f, 600f };
            float damage = baseDamage[Ashe.Spells[3].CastInfo.SpellLevel - 1] + (Ashe.Stats.AbilityPower.Total * 1.0f);

            if (target is Champion)
            {
                missile.SetToRemove();
                // Stun 1-3.5s based on distance traveled (simplified to 2s)
                AddBuff("Stun", 2f, 1, Arrow, target, Ashe, false);
                AddParticleTarget(Ashe, target, "Ashe_Base_R_tar.troy", target, lifetime: 1f);
            }
            target.TakeDamage(Ashe, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
        }
    }
}
