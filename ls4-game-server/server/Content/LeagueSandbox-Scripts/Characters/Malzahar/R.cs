using System.Numerics;
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
    /// Malzahar R - Nether Grasp
    /// 释放虚空之手束缚敌人
    /// </summary>
    public class MalzaharR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;

            // 魔法伤害: 200/350/500 (+1.3 AP)
            float[] baseDamage = { 200f, 350f, 500f };
            float ap = owner.Stats.AbilityPower.Total * 1.3f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 束缚
            AddBuff("MalzaharR", 2.5f, 1, spell, target, owner);

            AddParticle(owner, owner, "MalzaharR_cas", target.Position);
        }
    }
}
