using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class GravesBasicAttack : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            NotSingleTargetSpell = false,
            TriggersSpellCasts = false,
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(target.Position, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;
            float damage = owner.Stats.AttackDamage.Total;
            target.TakeDamage(owner, damage, GameServerCore.Enums.DamageType.DAMAGE_TYPE_PHYSICAL,
                GameServerCore.Enums.DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }
        public void OnSpellPostCast(Spell spell) { }
        public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }

    public class GravesBasicAttack2 : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            NotSingleTargetSpell = false,
            TriggersSpellCasts = false,
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(target.Position, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;
            float damage = owner.Stats.AttackDamage.Total;
            target.TakeDamage(owner, damage, GameServerCore.Enums.DamageType.DAMAGE_TYPE_PHYSICAL,
                GameServerCore.Enums.DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }
        public void OnSpellPostCast(Spell spell) { }
        public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }

    public class GravesCritAttack : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            NotSingleTargetSpell = false,
            TriggersSpellCasts = false,
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(target.Position, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;
            float critDamage = owner.Stats.AttackDamage.Total * owner.Stats.CriticalDamage.Total;
            target.TakeDamage(owner, critDamage, GameServerCore.Enums.DamageType.DAMAGE_TYPE_PHYSICAL,
                GameServerCore.Enums.DamageSource.DAMAGE_SOURCE_ATTACK, true);
        }
        public void OnSpellPostCast(Spell spell) { }
        public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
