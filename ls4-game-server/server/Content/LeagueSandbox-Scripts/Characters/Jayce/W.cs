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
    /// Jayce W - Mercury Cannon / Hammer
    /// 墨丘利之锤/炮
    /// </summary>
    public class JayceW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 持续时间: 4/5/6/7/8秒
            float[] duration = { 4f, 5f, 6f, 7f, 8f };
            float dur = duration[spell.CastInfo.SpellLevel - 1];

            AddBuff("JayceW", dur, 1, spell, owner, owner);

            AddParticle(owner, owner, "JayceW_cas", owner.Position);
        }
    }
}
