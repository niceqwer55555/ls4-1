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
    /// Nidalee R - Aspect of the Cougar
    /// 在人类形态和美洲豹形态之间切换
    /// 人类形态: 远程，使用标枪、陷阱和治疗
    /// 美洲豹形态: 近战，使用跳击、爪击和拍击
    /// </summary>
    public class AspectoftheCougar : ISpellScript
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

            // 形态转换 - 在cougar和human之间切换
            // 这个实现取决于游戏中是否有形态标志
            // 简单实现: 添加一个buff来表示cougar形态

            // 获得护甲和魔抗加成
            float[] armorBonus = { 10f, 20f, 30f };
            float[] mrBonus = { 10f, 20f, 30f };
            float armor = armorBonus[spell.CastInfo.SpellLevel - 1];
            float mr = mrBonus[spell.CastInfo.SpellLevel - 1];

            // 添加护甲和魔抗buff
            AddBuff("ArmorBuff", 5f, 1, spell, owner, owner);
            AddBuff("MagicResistBuff", 5f, 1, spell, owner, owner);

            // 添加移速buff
            AddBuff("MovementSpeedBuff", 5f, 1, spell, owner, owner);

            AddParticle(owner, owner, "AspectoftheCougar_cas", owner.Position);
        }
    }
}
