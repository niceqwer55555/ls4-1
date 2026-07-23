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
    /// Trundle W - Frozen Domain
    /// 冰冻领域
    /// </summary>
    public class TrundleW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 攻速加成: 30/35/40/45/50%
            float[] attackSpeedBonus = { 0.30f, 0.35f, 0.40f, 0.45f, 0.50f };
            float bonus = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 5秒
            AddBuff("TrundleW", 5f, 1, spell, owner, owner);

            AddParticle(owner, owner, "TrundleW_cas", owner.Position);
        }
    }
}
