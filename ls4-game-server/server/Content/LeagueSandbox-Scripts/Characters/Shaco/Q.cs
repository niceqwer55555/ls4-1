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
    /// Shaco Q - Deceive
    /// 闪烁到目标位置并进入隐身，下次攻击造成暴击
    /// </summary>
    public class ShacoQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 位移到目标位置
            TeleportTo(owner, targetPos.X, targetPos.Y);

            // 隐身3.5秒
            AddBuff("ShacoQ", 3.5f, 1, spell, owner, owner);

            // 下次攻击暴击
            AddBuff("ShacoQAttack", 6f, 1, spell, owner, owner);

            AddParticle(owner, owner, "ShacoQ_cas", targetPos);
        }
    }
}
