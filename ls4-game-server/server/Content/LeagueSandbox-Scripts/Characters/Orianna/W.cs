using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;

namespace Spells
{
    /// <summary>
    /// Orianna W - Command Dissonance
    /// 命令球体释放能量波，伤害并减速敌人
    /// </summary>
    public class OriannaW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 魔法伤害: 75/115/155/195/235 (+0.7 AP)
            float[] baseDamage = { 75f, 115f, 155f, 195f, 235f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 减速: 20/25/30/35/40%
            float[] slowAmount = { 0.20f, 0.25f, 0.30f, 0.35f, 0.40f };
            float slow = slowAmount[spell.CastInfo.SpellLevel - 1];

            foreach (var unit in GetUnitsInRange(owner.Position, 400f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Slow", 2f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "OriannaW_cas", owner.Position);
        }
    }
}
