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
    public class RengarR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            float[] duration = { 12f, 16f, 20f };
            float dur = duration[spell.CastInfo.SpellLevel - 1];

            float[] speedBonus = { 0.4f, 0.5f, 0.6f };
            float speed = speedBonus[spell.CastInfo.SpellLevel - 1];

            AddBuff("RengarR", dur, 1, spell, owner, owner);
            AddBuff("MovementSpeed", dur, 1, spell, owner, owner);

            AddParticle(owner, owner, "RengarR_cas", owner.Position);
        }
    }
}