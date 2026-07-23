using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    /// <summary>
    /// Twitch Q - Ambush
    /// After 1 second, becomes stealthed for 4/5/6/7/8 seconds
    /// Gains 20% movement speed while stealthed
    /// </summary>
    public class TwitchHideInShadows : ISpellScript
    {
        ObjAIBase Twitch;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false
        };

        public void OnSpellPostCast(Spell spell)
        {
            Twitch = spell.CastInfo.Owner as Champion;
            OverrideAnimation(Twitch, "Run_STEALTH", "Run");
            spell.SetCooldown(0, true);
            CreateTimer(1, () =>
            {
                Twitch.SetTargetUnit(null, true);
                // Stealth duration: 4/5/6/7/8 seconds
                float[] stealthDuration = { 4f, 5f, 6f, 7f, 8f };
                float duration = stealthDuration[spell.CastInfo.SpellLevel - 1];
                AddBuff("TwitchHideInShadows", duration, 1, spell, Twitch, Twitch);
            });
        }
    }
}
