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
    /// Olaf R - Ragnarok
    /// 移除身上所有控制效果，免疫限制技能
    /// 被动获得护甲和魔抗加成
    /// </summary>
    public class Ragnarok : ISpellScript
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

            // 护甲加成: 30/45/60
            // 魔抗加成: 30/45/60
            float[] armorBonus = { 30f, 45f, 60f };
            float[] mrBonus = { 30f, 45f, 60f };

            float armor = armorBonus[spell.CastInfo.SpellLevel - 1];
            float mr = mrBonus[spell.CastInfo.SpellLevel - 1];

            // 添加护甲和魔抗buff
            AddBuff("ArmorBuff", 6f, 1, spell, owner, owner);
            AddBuff("MagicResistBuff", 6f, 1, spell, owner, owner);

            // 添加免疫控制buff
            AddBuff("CrowdControlImmunity", 6f, 1, spell, owner, owner);

            // 添加移动速度buff（跑向敌方英雄时）
            AddBuff("MovementSpeedBuff", 1f, 1, spell, owner, owner);

            AddParticle(owner, owner, "Ragnarok_cas", owner.Position);
        }
    }
}
