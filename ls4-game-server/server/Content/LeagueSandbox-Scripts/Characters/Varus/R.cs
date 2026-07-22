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
    /// Varus R - Chain of Corruption
    /// 释放一股腐败之力，对首个命中的敌人造成魔法伤害并眩晕
    /// 腐败之力会向附近的其他敌人蔓延，眩晕他们并继续蔓延
    /// </summary>
    public class VarusR : ISpellScript
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

            // 魔法伤害: 150/250/350 (+0.8 AP)
            float[] baseDamage = { 150f, 250f, 350f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            var target = spell.CastInfo.Targets[0].Unit;
            if (target != null)
            {
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                
                // 眩晕2秒
                AddBuff("VarusRStun", 2f, 1, spell, target, owner);
                
                // 添加腐败标记
                AddBuff("VarusRCorruption", 3f, 1, spell, target, owner);
            }

            AddParticle(owner, owner, "VarusR_cas", target.Position);
        }
    }
}
