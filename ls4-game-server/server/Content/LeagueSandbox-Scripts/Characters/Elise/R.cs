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
    /// Elise R - Spider Form
    /// 变身蜘蛛形态
    /// </summary>
    public class EliseR : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 额外伤害: 10/15/20 (+0.3 AP)
            float[] bonusDamage = { 10f, 15f, 20f };
            float ap = owner.Stats.AbilityPower.Total * 0.3f;
            float damage = bonusDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 持续时间: 6秒
            AddBuff("EliseR", 6f, 1, spell, owner, owner);
        }
    }
}
