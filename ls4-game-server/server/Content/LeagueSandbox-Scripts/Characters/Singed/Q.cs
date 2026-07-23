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
    /// Singed Q - Poison Trail
    /// 留下毒迹，对经过的敌人造成持续魔法伤害
    /// </summary>
    public class SingedQ : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 每秒魔法伤害: 44/68/92/116/140 (+0.5 AP)
            float[] dpsDamage = { 44f, 68f, 92f, 116f, 140f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float dps = dpsDamage[spell.CastInfo.SpellLevel - 1] + ap;

            // 毒迹持续3秒，每0.5秒造成一次伤害
            AddBuff("PoisonTrail", 3f, 1, spell, owner, owner);
        }
    }
}
