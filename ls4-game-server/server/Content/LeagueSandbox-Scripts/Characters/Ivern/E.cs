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
    /// Ivern E - Moonfeed
    /// 为友军提供护盾
    /// </summary>
    public class IvernE : ISpellScript
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

            // 持续时间: 3秒
            AddBuff("IvernE", 3f, 1, spell, target, owner);

            AddParticle(owner, owner, "IvernE_cas", target.Position);
        }
    }
}
