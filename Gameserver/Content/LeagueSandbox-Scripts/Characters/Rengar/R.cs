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
    /// Rengar R - Thrill of the Hunt
    /// 进入隐身状态并获得移动速度加成
    /// </summary>
    public class RengarR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 持续时间: 7/9/11秒
            float[] duration = { 7f, 9f, 11f };
            float dur = duration[spell.CastInfo.SpellLevel - 1];

            // 移动速度加成: 25/35/45%
            float[] speedBonus = { 0.25f, 0.35f, 0.45f };
            float speed = speedBonus[spell.CastInfo.SpellLevel - 1];

            AddBuff("RengarR", dur, 1, spell, owner, owner);

            AddParticle(owner, owner, "RengarR_cas", owner.Position);
        }
    }
}
