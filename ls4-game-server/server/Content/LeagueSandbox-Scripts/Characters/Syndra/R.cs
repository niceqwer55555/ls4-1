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
    /// <summary>Syndra R - 能量倾泻: 释放所有黑暗法球轰击目标造成大量伤害</summary>
    public class SyndraR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { TriggersSpellCasts = true, CastingBreaksStealth = true, IsDamagingSpell = true };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(target.Position, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var target = spell.CastInfo.Targets[0].Unit;
            if (target == null) return;

            // 总伤害: 270/405/540 + 45% AP
            float[] baseDamage = { 270f, 405f, 540f };
            float ap = owner.Stats.AbilityPower.Total * 0.45f;
            float totalDmg = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            AddParticleTarget(owner, target, "Syndra_R_cas.troy", target, lifetime: 2f, size: 1.2f);
            target.TakeDamage(owner, totalDmg, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
        }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
