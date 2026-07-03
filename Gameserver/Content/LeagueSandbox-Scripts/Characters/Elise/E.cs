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
    /// Elise E - Rappel
    /// 跳跃到目标位置
    /// </summary>
    public class EliseE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            // 眩晕: 1/1.25/1.5/1.75/2秒
            float[] stunDuration = { 1f, 1.25f, 1.5f, 1.75f, 2f };
            float stun = stunDuration[spell.CastInfo.SpellLevel - 1];

            // 移至目标位置
            TeleportTo(owner, targetPos.X, targetPos.Y);

            foreach (var unit in GetUnitsInRange(targetPos, 200f, true))
            {
                if (unit.Team != owner.Team)
                {
                    AddBuff("Stun", stun, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "EliseE_cas", targetPos);
        }
    }
}
