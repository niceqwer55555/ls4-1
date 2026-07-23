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
    /// Nunu W - Vitality
    /// 增加友军移动速度和生命回复
    /// </summary>
    public class NunuW : ISpellScript
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

            // 移动速度加成: 30/35/40/45/50%
            float[] speedBonus = { 0.30f, 0.35f, 0.40f, 0.45f, 0.50f };
            float speed = speedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 5秒
            AddBuff("NunuW", 5f, 1, spell, target, owner);
        }
    }
}
