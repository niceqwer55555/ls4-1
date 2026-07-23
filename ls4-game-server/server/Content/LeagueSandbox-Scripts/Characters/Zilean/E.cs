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
    /// Zilean E - Time Warp
    /// 为友军提供移动速度加成
    /// </summary>
    public class ZileanE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;

            // 移动速度加成: 40/55/70/85/100%
            float[] speedBonus = { 0.40f, 0.55f, 0.70f, 0.85f, 1.00f };
            float speed = speedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 2.5秒
            AddBuff("ZileanE", 2.5f, 1, spell, target, owner);
        }
    }
}
