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
    /// Orianna E - Command Protect
    /// 命令球体附着到友军身上，提供护盾
    /// </summary>
    public class OriannaE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var target = spell.CastInfo.Targets[0].Unit;

            // 护盾值: 80/120/160/200/240 (+0.4 AP)
            float[] shieldAmount = { 80f, 120f, 160f, 200f, 240f };
            float ap = owner.Stats.AbilityPower.Total * 0.4f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 持续时间: 4秒
            AddBuff("Shield", 4f, 1, spell, target, owner);

            AddParticle(owner, owner, "OriannaE_cas", target.Position);
        }
    }
}
