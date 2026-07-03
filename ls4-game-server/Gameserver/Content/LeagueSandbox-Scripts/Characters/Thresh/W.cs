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
    /// Thresh W - Dark Passage
    /// 黑暗通道
    /// </summary>
    public class ThreshW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护盾值: 50/75/100/125/150 (+0.4 AP)
            float[] shieldAmount = { 50f, 75f, 100f, 125f, 150f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 持续时间: 4秒
            AddBuff("ThreshW", 4f, 1, spell, owner, owner);

            AddParticle(owner, owner, "ThreshW_cas", owner.Position);
        }
    }
}
