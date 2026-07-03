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
    /// Seraphine W - Surround Sound
    /// 为周围友军提供护盾和移速
    /// </summary>
    public class SeraphineW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护盾值: 70/90/110/130/150 (+0.3 AP)
            float[] shieldAmount = { 70f, 90f, 110f, 130f, 150f };
            float ap = owner.Stats.AbilityPower.Total * 0.3f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 移动速度加成: 20%
            float speedBonus = 0.20f;

            // 持续时间: 3秒
            AddBuff("SeraphineW", 3f, 1, spell, owner, owner);

            AddParticle(owner, owner, "SeraphineW_cas", owner.Position);
        }
    }
}
