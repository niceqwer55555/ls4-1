using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class GnarE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            float[] baseDamage = { 40f, 50f, 60f, 70f, 80f };
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1];

            float[] speedBonus = { 0.2f, 0.3f, 0.4f, 0.5f, 0.6f };
            AddBuff("MoveSpeed", 3f, 1, spell, owner, owner);

            var trueCoords = GetPointFromUnit(owner, 525f);
            ForceMovement(owner, "Spell3", trueCoords, 1200, 0, 0, 0);

            AddParticle(owner, owner, "GnarE_cas", owner.Position);
        }
    }
}