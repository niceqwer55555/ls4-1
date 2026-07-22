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
    /// <summary>Graves R - Collateral Damage: 发射爆破弹造成大量伤害并后座力</summary>
    public class GravesChargeShot : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { TriggersSpellCasts = true, CastingBreaksStealth = true, IsDamagingSpell = true, NotSingleTargetSpell = true };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(start, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var castPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var dir = Vector2.Normalize(castPos - owner.Position);

            AddParticle(owner, null, "Graves_ChargeShot_tar.troy", owner.Position, lifetime: 0.5f);

            // 伤害公式: 250/400/550 + 1.5 bonus AD
            float[] baseDamage = { 250f, 400f, 550f };
            float bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + bonusAd * 1.5f;

            foreach (var u in GetUnitsInRange(owner.Position, 1000f, true))
            {
                if (u.Team != owner.Team)
                {
                    var toUnit = Vector2.Normalize(u.Position - owner.Position);
                    if (Vector2.Dot(dir, toUnit) > 0.8f) // 窄锥形
                    {
                        u.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL,
                            DamageSource.DAMAGE_SOURCE_SPELL, false);
                        AddParticleTarget(owner, u, "Graves_ChargeShot_tar2.troy", u, lifetime: 0.5f);
                    }
                }
            }

            // 后座力 - 击退自己
            var knockback = owner.Position - dir * 400f;
            TeleportTo(owner, knockback.X, knockback.Y);
        }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
