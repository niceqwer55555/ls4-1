using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class Ragnarok : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            float[] armorBonus = { 40f, 60f, 80f };
            float extraAd = owner.Stats.AttackDamage.FlatBonus;
            float armor = armorBonus[spell.CastInfo.SpellLevel - 1] + extraAd * 0.3f;

            AddBuff("Ragnarok", 6f, 1, spell, owner, owner);

            AddParticle(owner, owner, "Ragnarok_cas", owner.Position);
        }
    }
}