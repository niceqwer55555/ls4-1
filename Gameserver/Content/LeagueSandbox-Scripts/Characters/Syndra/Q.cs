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
    /// <summary>Syndra Q - 暗黑法球: 在目标位置召唤一个黑暗法球造成伤害</summary>
    public class SyndraQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { TriggersSpellCasts = true, CastingBreaksStealth = true, IsDamagingSpell = true, NotSingleTargetSpell = true };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(start, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var p = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            AddParticle(owner, null, "Syndra_Q_sphere.troy", p, 8f, 0.8f);
            AddParticlePos(owner, "Syndra_Q_mis.troy", owner.Position, p, 0.5f);

            // 伤害公式: 70/105/140/175/210 + 70% AP
            float[] baseDamage = { 70f, 105f, 140f, 175f, 210f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float dmg = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;
            foreach (var u in GetUnitsInRange(p, 200f, true))
            {
                if (u.Team != owner.Team)
                    u.TakeDamage(owner, dmg, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            }

            AddBuff("SyndraQCount", 8f, 1, spell, owner, owner);
        }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }

    /// <summary>Syndra Q spell missile effect</summary>
    public class SyndraQSpell : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { NotSingleTargetSpell = true };
        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end) { }
        public void OnSpellCast(Spell spell) { }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
