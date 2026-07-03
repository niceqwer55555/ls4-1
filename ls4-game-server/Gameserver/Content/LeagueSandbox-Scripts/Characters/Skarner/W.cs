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
    /// Skarner W - Crystalline Exoskeleton
    /// 获得护盾和移动速度
    /// </summary>
    public class SkarnerW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护盾值: 60/90/120/150/180 (+0.6 AP)
            float[] shieldAmount = { 60f, 90f, 120f, 150f, 180f };
            float ap = owner.Stats.AbilityPower.Total * 0.6f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 移动速度加成: 10/15/20/25/30%
            float[] speedBonus = { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f };
            float speed = speedBonus[spell.CastInfo.SpellLevel - 1];

            // 持续时间: 6秒
            AddBuff("SkarnerW", 6f, 1, spell, owner, owner);
        }
    }
}
