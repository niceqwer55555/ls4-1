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
    /// <summary>Syndra E - 弱者退散: 锥形范围击退+伤害，命中球体造成眩晕</summary>
    public class SyndraE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { TriggersSpellCasts = true, CastingBreaksStealth = true, IsDamagingSpell = true, NotSingleTargetSpell = true };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(start, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var p = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var dir = Vector2.Normalize(p - owner.Position);
            AddParticle(owner, null, "Syndra_E_cas.troy", owner.Position, direction: new Vector3(dir.X, 0, dir.Y), lifetime: 1f);

            // 伤害公式: 85/130/175/220/265 + 0.6 AP
            float[] baseDamage = { 85f, 130f, 175f, 220f, 265f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float dmg = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;
            foreach (var u in GetUnitsInRange(owner.Position, 650f, true))
            {
                if (u.Team != owner.Team)
                {
                    var toUnit = Vector2.Normalize(u.Position - owner.Position);
                    if (Vector2.Dot(dir, toUnit) > 0.4f) // 锥形检测
                    {
                        u.TakeDamage(owner, dmg, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                        AddBuff("Stun", 1.5f, 1, spell, u, owner);
                        // 击退
                        TeleportTo((ObjAIBase)u, u.Position.X + dir.X * 300, u.Position.Y + dir.Y * 300);
                    }
                }
            }
        }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
