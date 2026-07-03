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
    /// <summary>Syndra W - 驱使念力: 抓取并投掷目标区域敌人造成伤害和减速</summary>
    public class SyndraW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        { TriggersSpellCasts = true, CastingBreaksStealth = true, IsDamagingSpell = true, NotSingleTargetSpell = true };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        { FaceDirection(start, owner, true); }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var p = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            AddParticlePos(owner, "Syndra_W_cas.troy", owner.Position, p, 0.8f);

            // 伤害公式: 70/110/150/190/230 + 0.7 AP
            float[] baseDamage = { 70f, 110f, 150f, 190f, 230f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float dmg = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;
            foreach (var u in GetUnitsInRange(p, 225f, true))
            {
                if (u.Team != owner.Team)
                {
                    u.TakeDamage(owner, dmg, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                    AddBuff("Slow", 1.5f, 1, spell, u, owner);
                }
            }
        }
        public void OnSpellPostCast(Spell spell) { } public void OnUpdate(float diff) { }
        public void OnActivate(ObjAIBase owner, Spell spell) { } public void OnDeactivate(ObjAIBase owner, Spell spell) { }
    }
}
