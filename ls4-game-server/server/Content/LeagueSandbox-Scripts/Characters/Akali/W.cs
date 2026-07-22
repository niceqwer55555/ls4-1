using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class AkaliW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var pos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            float[] duration = { 5f, 5.5f, 6f, 6.5f, 7f };
            float[] speedBonus = { 0.3f, 0.35f, 0.4f, 0.45f, 0.5f };

            AddBuff("AkaliSmokeBomb", duration[spell.CastInfo.SpellLevel - 1], 1, spell, owner, owner);
            AddBuff("MoveSpeed", duration[spell.CastInfo.SpellLevel - 1], 1, spell, owner, owner);

            AddParticle(owner, null, "AkaliW_cas", pos);
        }
    }
}