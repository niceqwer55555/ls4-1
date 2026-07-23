using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.SpellNS.Missile;

namespace Spells
{
    /// <summary>
    /// Graves W - Smoke Screen
    /// 发射烟雾弹，造成魔法伤害并减速敌人
    /// </summary>
    public class GravesSmokeGrenade : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void ApplyEffects(ObjAIBase owner, AttackableUnit target, Spell spell, SpellMissile missile)
        {
            // 伤害公式: 60/110/160/210/260 + 0.6 AP
            float[] baseDamage = { 60f, 110f, 160f, 210f, 260f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // 减速4秒
            AddBuff("Slow", 4f, 1, spell, target, owner);

            missile.SetToRemove();
        }
    }
}
