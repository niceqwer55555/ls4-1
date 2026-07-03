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
    /// Sivir R - On the Hunt
    /// 获得攻击速度和移动速度加成
    /// 周围友军获得一半攻击速度和全部移动速度加成
    /// </summary>
    public class OntheHunt : ISpellScript
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

            // 攻击速度加成: 30/45/60%
            float[] attackSpeedBonus = { 0.30f, 0.45f, 0.60f };
            // 移动速度加成: 20%
            float moveSpeedBonus = 0.20f;

            float attackSpeed = attackSpeedBonus[spell.CastInfo.SpellLevel - 1];

            // 为自己添加攻击速度和移动速度buff
            AddBuff("AttackSpeedBuff", 10f, 1, spell, owner, owner);
            AddBuff("MovementSpeedBuff", 10f, 1, spell, owner, owner);

            // 为范围内的友军添加移动速度buff
            foreach (var unit in GetUnitsInRange(owner.Position, 1000f, true))
            {
                if (unit.Team == owner.Team && unit is Champion && unit != owner)
                {
                    AddBuff("MovementSpeedBuff", 10f, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "OntheHunt_cas", owner.Position);
        }
    }
}
