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
    /// Nocturne E - Paranoia
    /// 减少附近敌方英雄的视野范围
    /// </summary>
    public class NocturneE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 视野减少: 50/100/150/200/250
            float[] visionReduction = { 50f, 100f, 150f, 200f, 250f };
            float reduction = visionReduction[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 5秒
            AddBuff("NocturneE", 5f, 1, spell, owner, owner);
        }
    }
}
