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
    /// Sivir E - Spell Shield
    /// 创建一个魔法屏障，阻挡单个敌方技能
    /// 如果成功阻挡，恢复150魔法值
    /// </summary>
    public class SpellShield : ISpellScript
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

            // 添加持续3秒的法术护盾
            AddBuff("SpellShield", 3f, 1, spell, owner, owner);

            // 如果护盾被触发，恢复150魔法值
            // 这个效果通常在buff被消耗时处理

            AddParticle(owner, owner, "SpellShield_cas", owner.Position);
        }
    }
}
