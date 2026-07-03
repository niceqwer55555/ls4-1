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
    /// Udyr Q - Wildcard
    /// 获得攻击速度加成
    /// </summary>
    public class UdyrQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 攻击速度加成: 20/30/40/50/60%
            float[] attackSpeedBonus = { 0.20f, 0.30f, 0.40f, 0.50f, 0.60f };
            float speed = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 5秒
            AddBuff("UdyrQ", 5f, 1, spell, owner, owner);
        }
    }
}
