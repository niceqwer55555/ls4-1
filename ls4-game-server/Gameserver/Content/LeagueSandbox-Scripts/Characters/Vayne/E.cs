using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;
using LeagueSandbox.GameServer.API;
using System.Numerics;
using GameServerLib.GameObjects.AttackableUnits;

namespace Spells
{
    public class VayneCondemn : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, false, target, Vector2.Zero);
        }
    }

    public class VayneCondemnMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
        };
        ObjAIBase Owner;

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            Owner = owner;
        }
        public void OnSpellPostCast(Spell spell)
        {
            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;

            FaceDirection(Owner.Position, target);

            var spellLevel = spell.CastInfo.SpellLevel;
            float[] baseDamage = { 50f, 85f, 120f, 155f, 190f };
            float adRatio = Owner.Stats.AttackDamage.Total * 1.0f;
            float damage = baseDamage[spellLevel - 1] + adRatio;

            target.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            var knockbackPos = GetPointFromUnit(target, 450f);
            ForceMovement(target, "RUN", knockbackPos, 2200, 0, 0, 0);

            AddParticleTarget(Owner, target, "Vayne_Base_E_tar.troy", target);
        }
    }
}