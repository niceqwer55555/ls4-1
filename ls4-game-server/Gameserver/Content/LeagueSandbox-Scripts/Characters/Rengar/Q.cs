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
    public class RengarQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            float[] bonusDamage = { 30f, 60f, 90f, 120f, 150f };
            float ad = owner.Stats.AttackDamage.Total;
            float damage = bonusDamage[spell.CastInfo.SpellLevel - 1] + ad;

            float[] attackSpeed = { 0.3f, 0.35f, 0.4f, 0.45f, 0.5f };

            AddBuff("RengarQ", 3f, 1, spell, owner, owner);

            AddParticle(owner, owner, "RengarQ_cas", owner.Position);
        }
    }
}