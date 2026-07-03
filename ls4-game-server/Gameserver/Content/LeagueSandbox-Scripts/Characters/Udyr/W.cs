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
    /// Udyr W - Iron Will
    /// 获得护甲和生命回复
    /// </summary>
    public class UdyrW : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            // 护甲加成: 20/30/40/50/60
            float[] armorBonus = { 20f, 30f, 40f, 50f, 60f };

            // 生命回复: 100/150/200/250/300
            float[] healAmount = { 100f, 150f, 200f, 250f, 300f };

            // 持续时间: 5秒
            AddBuff("UdyrW", 5f, 1, spell, owner, owner);
        }
    }
}
