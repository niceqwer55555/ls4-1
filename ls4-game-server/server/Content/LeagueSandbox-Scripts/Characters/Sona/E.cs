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
    /// Sona E - Song of Celerity
    /// 给予自身和附近友军移动速度加成
    /// </summary>
    public class SonaE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 移动速度加成: 20/25/30/35/40% (+3% per 100 AP)
            float[] moveSpeedBonus = { 0.20f, 0.25f, 0.30f, 0.35f, 0.40f };
            float bonus = moveSpeedBonus[spell.CastInfo.SpellLevel - 1];
            float ap = owner.Stats.AbilityPower.Total * 0.03f / 100f;
            float totalBonus = bonus + ap;

            // 持续3秒，脱离战斗后延长至7秒
            float duration = 3f;

            // 给自身加速
            AddBuff("MoveSpeedBuff", duration, 1, spell, owner, owner);

            // 给附近友军加速
            foreach (var unit in GetUnitsInRange(owner.Position, 1000f, true))
            {
                if (unit.Team == owner.Team && unit is Champion)
                {
                    AddBuff("MoveSpeedBuff", duration, 1, spell, unit, owner);
                }
            }

            AddParticle(owner, owner, "SonaE_cas", owner.Position);
        }
    }
}
