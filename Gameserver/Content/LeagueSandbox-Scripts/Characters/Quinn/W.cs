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
    /// Quinn W - Heightened Senses
    /// 被动暴露附近隐形敌人，主动使用获得攻击速度
    /// </summary>
    public class QuinnW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 攻击速度加成: 25/35/45/55/65%
            float[] attackSpeedBonus = { 0.25f, 0.35f, 0.45f, 0.55f, 0.65f };
            float speed = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 5秒
            AddBuff("QuinnW", 5f, 1, spell, owner, owner);
        }
    }
}
