using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class SyndraBasicAttack : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { NotSingleTargetSpell = false, TriggersSpellCasts = false };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(target.Position, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var t = spell.CastInfo.Targets[0].Unit;
            if (t == null) return;
            t.TakeDamage(spell.CastInfo.Owner, spell.CastInfo.Owner.Stats.AttackDamage.Total,
                DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }
        public void OnSpellPostCast(Spell spell) { }
        public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { }
        public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraBasicAttack2 : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { NotSingleTargetSpell = false, TriggersSpellCasts = false };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(target.Position, owner, true); }
        public void OnSpellCast(Spell spell)
        {
            var t = spell.CastInfo.Targets[0].Unit;
            if (t == null) return;
            t.TakeDamage(spell.CastInfo.Owner, spell.CastInfo.Owner.Stats.AttackDamage.Total,
                DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraCritAttack : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { NotSingleTargetSpell = false, TriggersSpellCasts = false };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(target.Position, owner, true); }
        public void OnSpellCast(Spell spell)
        {
            var t = spell.CastInfo.Targets[0].Unit;
            if (t == null) return;
            t.TakeDamage(spell.CastInfo.Owner, spell.CastInfo.Owner.Stats.AttackDamage.Total * 2f,
                DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, true);
        }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
