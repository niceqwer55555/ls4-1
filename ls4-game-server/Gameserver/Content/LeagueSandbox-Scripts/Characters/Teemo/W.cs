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
    public class MoveQuick : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            
            // 被动移动速度加成: 10/14/18/22/26%
            float[] passiveMoveSpeed = { 0.10f, 0.14f, 0.18f, 0.22f, 0.26f };
            
            // 主动移动速度加成: 20/28/36/44/52%
            float[] activeMoveSpeed = { 0.20f, 0.28f, 0.36f, 0.44f, 0.52f };
            
            float passiveSpeed = passiveMoveSpeed[spell.CastInfo.SpellLevel - 1];
            float activeSpeed = activeMoveSpeed[spell.CastInfo.SpellLevel - 1];
            
            // 添加被动移动速度buff
            AddBuff("MoveQuickPassive", float.MaxValue, 1, spell, owner, owner);
            
            // 添加主动冲刺buff
            AddBuff("MoveQuickActive", 3f, 1, spell, owner, owner);
        }
    }
}
