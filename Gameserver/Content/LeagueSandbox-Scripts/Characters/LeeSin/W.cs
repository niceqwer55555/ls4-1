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
    /// LeeSin W - Safeguard
    /// 突进到友军旁边，提供护盾
    /// </summary>
    public class Safeguard : ISpellScript
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

            // 护盾公式: 40/80/120/160/200 + 0.8 AP
            float[] shieldAmount = { 40f, 80f, 120f, 160f, 200f };
            float ap = owner.Stats.AbilityPower.Total * 0.8f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 添加护盾buff
            AddBuff("Shield", 2f, 1, spell, owner, owner);

            AddParticle(owner, owner, "Safeguard_cas", owner.Position);
        }
    }
}
