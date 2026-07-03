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
    /// Nautilus W - Titan's Wrath
    /// 泰坦之怒
    /// </summary>
    public class NautilusW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护盾值: 60/100/140/180/220 (+0.8 AP)
            float[] shieldAmount = { 60f, 100f, 140f, 180f, 220f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 持续时间: 4秒
            AddBuff("NautilusW", 4f, 1, spell, owner, owner);

            AddParticle(owner, owner, "NautilusW_cas", owner.Position);
        }
    }
}
