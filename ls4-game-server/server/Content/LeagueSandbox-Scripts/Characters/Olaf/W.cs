using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Olaf W - Vicious Strikes
    /// 6秒内获得攻击速度、生命偷取和法术吸血
    /// </summary>
    public class ViciousStrikes : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 攻击速度加成: 50/60/70/80/90%
            float[] attackSpeedBonus = { 0.50f, 0.60f, 0.70f, 0.80f, 0.90f };
            // 生命偷取和法术吸血: 16/18/20/22/24%
            float[] lifesteal = { 0.16f, 0.18f, 0.20f, 0.22f, 0.24f };

            float attackSpeed = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 添加攻击速度buff
            AddBuff("AttackSpeedBuff", 6f, 1, spell, owner, owner);

            // 添加生命偷取buff
            AddBuff("Lifesteal", 6f, 1, spell, owner, owner);

            AddParticle(owner, owner, "ViciousStrikes_cas", owner.Position);
        }
    }
}
