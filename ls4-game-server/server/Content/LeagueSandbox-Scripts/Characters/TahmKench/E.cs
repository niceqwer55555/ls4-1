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
    /// TahmKench E - Thick Skin
    /// 被动：受到伤害时累积层数，主动激活获得护盾
    /// </summary>
    public class TahmKenchE : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护盾值: 100/150/200/250/300 (+0.5 AP)
            float[] shieldAmount = { 100f, 150f, 200f, 250f, 300f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float shield = shieldAmount[spell.CastInfo.SpellLevel - 1] + ap;

            // 持续时间: 3秒
            AddBuff("TahmKenchE", 3f, 1, spell, owner, owner);
        }
    }
}
