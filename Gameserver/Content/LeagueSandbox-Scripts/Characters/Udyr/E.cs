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
    /// Udyr E - Wind Lee
    /// 获得移动速度加成
    /// </summary>
    public class UdyrE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 移动速度加成: 15/20/25/30/35%
            float[] speedBonus = { 0.15f, 0.20f, 0.25f, 0.30f, 0.35f };
            float speed = speedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 5秒
            AddBuff("UdyrE", 5f, 1, spell, owner, owner);
        }
    }
}
