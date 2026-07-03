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
    /// Rammus W - Defensive Ball Curl
    /// 缩成球形态，大幅增加护甲和魔抗，并反弹伤害
    /// </summary>
    public class RammusW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护甲和魔抗加成: 40/60/80/100/120
            float[] resistBonus = { 40f, 60f, 80f, 100f, 120f };
            float resist = resistBonus[spell.CastInfo.SpellLevel - 1];

            // 持续6秒
            AddBuff("RammusW", 6f, 1, spell, owner, owner);
        }
    }
}
