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
    /// RekSai W - Burrow
    /// 遁入地下获得不同效果（人类形态减速敌人，虚空形态获得护盾）
    /// </summary>
    public class RekSaiW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 移速减少/增加: 30/35/40/45/50%
            float[] speedBonus = { 0.30f, 0.35f, 0.40f, 0.45f, 0.50f };
            float speed = speedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 4秒
            AddBuff("RekSaiW", 4f, 1, spell, owner, owner);
        }
    }
}
