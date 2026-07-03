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
    /// Singed R - Insanity
    /// 大幅增加移动速度、生命值和法术强度
    /// </summary>
    public class SingedR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 额外生命值: 300/500/700
            float[] healthBonus = { 300f, 500f, 700f };
            float health = healthBonus[spell.CastInfo.SpellLevel - 1];

            // 法术强度加成: 50/75/100
            float[] apBonus = { 50f, 75f, 100f };
            float ap = apBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 8秒
            AddBuff("SingedR", 8f, 1, spell, owner, owner);

            AddParticle(owner, owner, "SingedR_cas", owner.Position);
        }
    }
}
