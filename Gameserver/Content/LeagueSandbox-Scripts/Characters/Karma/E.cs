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
    /// Karma E - Inspire
    /// 为友军提供护盾和移动速度
    /// </summary>
    public class KarmaE : ISpellScript
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

            // 护盾值: 80/110/140/170/200 (+0.6 AP)
            float[] shieldAmount = { 80f, 110f, 140f, 170f, 200f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 移动速度加成: 40%
            float speedBonus = 0.40f;

            // 持续时间: 2.5秒
            AddBuff("KarmaE", 2.5f, 1, spell, target, owner);

            AddParticle(owner, owner, "KarmaE_cas", target.Position);
        }
    }
}
