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
    public class SyndraEMissile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraEMissile2 : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraEMissile3 : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraRSpell : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraRBounce : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraRBounce5 : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraE5Missile : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraRTrigger : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraPassive : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraWCast : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
    public class SyndraWCDTimer : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata() { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
