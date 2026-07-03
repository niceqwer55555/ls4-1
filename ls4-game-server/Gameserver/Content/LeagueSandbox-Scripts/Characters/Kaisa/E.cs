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
    /// Kai'Sa E - Supercharge
    /// 超级充能
    /// </summary>
    public class KaisaE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 攻速加成
            float[] attackSpeedBonus = { 0.4f, 0.5f, 0.6f, 0.7f, 0.8f };
            float bonus = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 4秒
            AddBuff("KaisaE", 4f, 1, spell, owner, owner);

            AddParticle(owner, owner, "KaisaE_cas", owner.Position);
        }
    }
}
