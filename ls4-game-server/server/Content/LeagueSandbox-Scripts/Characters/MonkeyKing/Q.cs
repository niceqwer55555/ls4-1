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
    /// MonkeyKing Q - Crushing Blow
    /// 下次攻击造成额外物理伤害
    /// </summary>
    public class MonkeyKingQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 额外物理伤害: 30/60/90/120/150 (+0.15 AD)
            float[] bonusDamage = { 30f, 60f, 90f, 120f, 150f };
            float ad = owner.Stats.AttackDamage.Total;
            float damage = bonusDamage[spell.CastInfo.SpellLevel - 1] + ad * 0.15f;

            // 持续时间: 4秒
            AddBuff("MonkeyKingQ", 4f, 1, spell, owner, owner);
        }
    }
}
