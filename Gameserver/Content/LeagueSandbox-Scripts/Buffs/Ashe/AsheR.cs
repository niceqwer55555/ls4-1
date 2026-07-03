using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace Buffs
{
    /// <summary>
    /// Ashe R - Enchanted Crystal Arrow
    /// 冰箭击中后造成晕眩
    /// </summary>
    internal class AsheR : IBuffGameScript
    {
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.STUN,
            BuffAddType = BuffAddType.REPLACE_EXISTING,
            IsHidden = false
        };

        Particle stun;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            float[] stunDuration = { 1f, 1.5f, 2f };
            float duration = stunDuration[ownerSpell.CastInfo.SpellLevel - 1];
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(stun);
        }
    }
}
