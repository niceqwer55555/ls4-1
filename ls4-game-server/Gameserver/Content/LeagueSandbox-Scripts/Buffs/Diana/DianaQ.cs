using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Buffs
{
    /// <summary>
    /// Diana Q - Crescent Strike
    /// 标记效果
    /// </summary>
    internal class DianaQ : IBuffGameScript
    {

        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public BuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.HASTE,
            BuffAddType = BuffAddType.STACKS_AND_OVERLAPS,
            IsHidden = false
        };

        Particle mark;

        public void OnActivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            mark = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Diana_Q_Icon", unit, buff.Duration);
        }

        public void OnDeactivate(AttackableUnit unit, Buff buff, Spell ownerSpell)
        {
            RemoveParticle(mark);
        }
    }
}


