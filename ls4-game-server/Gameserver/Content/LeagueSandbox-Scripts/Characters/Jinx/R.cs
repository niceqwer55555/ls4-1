using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
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
    /// Jinx R - Super Mega Death Rocket!
    /// Damage: 25/30/35 (+10% AD) to 250/350/450 (+100% AD) based on distance
    /// Simplified: 125/175/225 (+50% AD) mid-range
    /// </summary>
    public class JinxR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            MissileParameters = new MissileParameters { Type = MissileType.Circle },
        };

        ObjAIBase _owner;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            _owner = owner;
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        private void TargetExecute(Spell spell, AttackableUnit target, SpellMissile missile, SpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            // Damage: 125/175/225 (+50% AD) simplified mid-range
            float[] baseDamage = { 125f, 175f, 225f };
            var ad = owner.Stats.AttackDamage.Total * 0.5f;
            var damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad;

            if (target is Champion)
            {
                AddBuff("JinxWSight", 2f, 1, spell, target, _owner);
            }

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            AddParticleTarget(owner, target, "Ezreal_mysticshot_tar", target);
            missile.SetToRemove();
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { }
        public void OnSpellChannel(Spell spell) { }
        public void OnSpellChannelCancel(Spell spell, ChannelingStopSource reason) { }
        public void OnSpellPostChannel(Spell spell) { }

        public void OnUpdate(float diff)
        {
            SetSpellToolTipVar(_owner, 0, _owner.Stats.AttackDamage.Total - _owner.Stats.AttackDamage.BaseValue, SpellbookType.SPELLBOOK_CHAMPION, 3, SpellSlotType.SpellSlots);
        }
    }
}
