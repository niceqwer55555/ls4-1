using System.Numerics;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Lux W - Prismatic Barrier
    /// 扔出魔棒保护所有碰触的友军免受伤害
    /// </summary>
    public class PrismaticBarrier : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护盾公式: 80/105/130/155/180 + 0.35 AP
            float[] shieldAmount = { 80f, 105f, 130f, 155f, 180f };
            float ap = owner.Stats.AbilityPower.Total * 0.35f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 对路径上的友军添加护盾
            foreach (var unit in GetUnitsInRange(owner.Position, 1075f, true))
            {
                if (unit.Team == owner.Team && unit is Champion)
                {
                    AddBuff("Shield", 3f, 1, spell, unit, owner);
                }
            }

            // 为自己添加护盾
            AddBuff("Shield", 3f, 1, spell, owner, owner);

            AddParticle(owner, owner, "PrismaticBarrier_cas", owner.Position);
        }
    }
}
