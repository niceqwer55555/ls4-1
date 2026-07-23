using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerLib.GameObjects.AttackableUnits;
using GameServerCore;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Sector;

namespace Spells
{
    /// <summary>
    /// Irelia W - Hiten Style
    /// Passive: Auto-attacks deal 5/7/9/11/13 (+15% AP) true damage and heal for 5/7/9/11/13
    /// Active: Auto-attacks deal 15/30/45/60/75 (+100% AP) true damage for 6 seconds
    /// </summary>
    public class IreliaHitenStyle : ISpellScript
    {
        ObjAIBase Irelia;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            Irelia = owner = spell.CastInfo.Owner as Champion;
            ApiEventManager.OnLevelUpSpell.AddListener(this, spell, OnLevelUp, true);
        }

        public void OnLevelUp(Spell spell)
        {
            // Passive buff - always active when spell is leveled
            AddBuff("IreliaHitenStyle", 250000f, 1, spell, Irelia, Irelia);
        }

        public void OnSpellCast(Spell spell)
        {
            Irelia.CancelAutoAttack(true);
            // Active: enhanced true damage on auto for 6 seconds
            AddBuff("IreliaHitenStyleCharged", 6.0f, 1, spell, Irelia, Irelia);
            if (Irelia.HasBuff("IreliaHitenStyle"))
            {
                Irelia.RemoveBuffsWithName("IreliaHitenStyle");
            }
        }
    }
}
