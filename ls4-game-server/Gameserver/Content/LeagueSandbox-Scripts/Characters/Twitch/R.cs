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
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using LeagueSandbox.GameServer.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Spells
{
    /// <summary>
    /// Twitch R - Spray and Pray
    /// Gain 300 attack range and 20/40/60 AD for 7 seconds
    /// Attacks pierce enemies dealing 20% reduced damage per hit
    /// </summary>
    public class TwitchFullAutomatic : ISpellScript
    {
        ObjAIBase Twitch;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellCast(Spell spell)
        {
            Twitch = spell.CastInfo.Owner as Champion;
            Twitch.CancelAutoAttack(true);
            // Duration: 7 seconds at all levels
            AddBuff("TwitchFullAutomatic", 7f, 1, spell, Twitch, Twitch);
        }
    }

    /// <summary>
    /// Twitch R Attack Missile - piercing attacks
    /// </summary>
    public class TwitchSprayandPrayAttack : ISpellScript
    {
        ObjAIBase Twitch;
        SpellMissile Missile;
        private List<AttackableUnit> UnitsHit = new List<AttackableUnit>();
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Twitch = spell.CastInfo.Owner as Champion;
            UnitsHit.Clear();
            Missile = spell.CreateSpellMissile(new MissileParameters
            {
                Type = MissileType.Circle,
            });
            ApiEventManager.OnSpellMissileHit.AddListener(this, Missile, TargetExecute, false);
        }

        public void TargetExecute(SpellMissile missile, AttackableUnit target)
        {
            if (UnitsHit.Contains(target)) return;
            UnitsHit.Add(target);

            float damage = Twitch.Stats.AttackDamage.Total;

            // Each pierce reduces damage by 20%
            for (int i = 1; i < UnitsHit.Count; i++)
            {
                damage *= 0.8f;
            }

            if (Twitch.IsNextAutoCrit)
            {
                target.TakeDamage(Twitch, damage * 2, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, true);
            }
            else
            {
                target.TakeDamage(Twitch, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            }
            AddParticleTarget(Twitch, target, "Twitch_Base_R_Tar.troy", target);
        }
    }
}
