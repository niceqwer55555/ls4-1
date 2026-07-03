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
    /// Janna E - Eye of the Storm
    /// 为友军提供护盾，护盾被破坏时为目标提供攻击力加成
    /// </summary>
    public class JannaE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;

            // 护盾值: 80/120/160/200/240 (+0.9 AP)
            float[] shieldAmount = { 80f, 120f, 160f, 200f, 240f };
            float ap = owner.Stats.AbilityPower.Total * 0.9f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            AddBuff("Shield", 5f, 1, spell, target, owner);

            // 攻击力加成: 10/17.5/25/32.5/40 (+0.1 AP)
            float[] adBonus = { 10f, 17.5f, 25f, 32.5f, 40f };
            float ad = adBonus[spell.CastInfo.SpellLevel - 1] + ap * 0.1f;
            AddBuff("AttackSpeedBuff", 5f, 1, spell, target, owner);
        }
    }
}
