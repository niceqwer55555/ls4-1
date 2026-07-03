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
    /// MonkeyKing W - Decoy
    /// 制造一个幻象分身
    /// </summary>
    public class MonkeyKingW : ISpellScript
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

            // 幻象持续时间: 4/5/6/7/8秒
            float[] duration = { 4f, 5f, 6f, 7f, 8f };
            float dur = duration[spell.CastInfo.SpellLevel - 1];

            // 移至目标位置
            TeleportTo(owner, targetPos.X, targetPos.Y);

            AddBuff("MonkeyKingW", dur, 1, spell, owner, owner);

            AddParticle(owner, owner, "MonkeyKingW_cas", targetPos);
        }
    }
}
