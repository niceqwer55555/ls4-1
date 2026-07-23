using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    /// <summary>
    /// Miss Fortune W - Strut
    /// 被动：增加移动速度
    /// 主动：立即刷新被动并获得额外移速
    /// </summary>
    public class MissFortuneStrut : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
        }

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 获得移动速度加成 - 持续4秒
            float moveSpeedBonus = 25f + (spell.CastInfo.SpellLevel * 15f);
            AddBuff("MoveSpeed", 4f, 1, spell, owner, owner);

            // 刷新被动层数
            AddBuff("MissFortunePassive", 4f, 1, spell, owner, owner);

            // 粒子效果
            AddParticle(owner, owner, "MissFortune_W_cas", owner.Position, lifetime: 0.5f);
        }

        public void OnSpellPostCast(Spell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
        }
    }
}