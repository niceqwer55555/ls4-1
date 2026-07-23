using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerLib.GameObjects.AttackableUnits;

namespace Spells
{
    /// <summary>
    /// Fiora E - Burst of Speed
    /// Grants bonus attack speed for 3 seconds
    /// Critical strikes and auto-attacks on champions extend the duration
    /// </summary>
    public class FioraFlurry : ISpellScript
    {
        Spell Flurry;
        ObjAIBase Fiora;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = false
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
            Flurry = spell;
            Fiora = owner = spell.CastInfo.Owner as Champion;
            AddBuff("FioraFlurry", 3.0f, 1, spell, Fiora, Fiora);
        }
    }
}
