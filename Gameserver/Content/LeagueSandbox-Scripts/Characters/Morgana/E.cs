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
    /// Morgana E - Black Shield
    /// 为友军提供免疫控制的护盾
    /// </summary>
    public class MorganaE : ISpellScript
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

            // 护盾值: 80/120/160/200/240 (+0.7 AP)
            float[] shieldAmount = { 80f, 120f, 160f, 200f, 240f };
            float ap = owner.Stats.AbilityPower.Total * 0.7f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 持续时间: 5秒
            AddBuff("MorganaE", 5f, 1, spell, target, owner);

            AddParticle(owner, owner, "MorganaE_cas", target.Position);
        }
    }
}
