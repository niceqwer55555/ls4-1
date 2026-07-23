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
    /// Kled W - Violence
    /// 获得攻击速度加成
    /// </summary>
    public class KledW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 攻击速度加成: 40/50/60/70/80%
            float[] attackSpeedBonus = { 0.40f, 0.50f, 0.60f, 0.70f, 0.80f };
            float speed = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 4秒
            AddBuff("KledW", 4f, 1, spell, owner, owner);
        }
    }
}
