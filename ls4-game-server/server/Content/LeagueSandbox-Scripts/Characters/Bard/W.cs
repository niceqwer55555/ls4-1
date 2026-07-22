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
    /// Bard W - Caretaker's Shrine
    /// 放置守护者神殿治疗友军
    /// </summary>
    public class BardW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 治疗量: 50/75/100/125/150 (+0.4 AP)
            float[] healAmount = { 50f, 75f, 100f, 125f, 150f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float heal = healAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 持续时间: 8秒
            float dur = 8f;

            AddBuff("BardW", dur, 1, spell, owner, owner);

            AddParticle(owner, owner, "BardW_cas", targetPos);
        }
    }
}
