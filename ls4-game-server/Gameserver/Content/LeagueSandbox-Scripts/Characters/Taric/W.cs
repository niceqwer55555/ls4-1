using System.Linq;
using GameServerCore;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace Spells
{
    public class Shatter : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner;
            var armor = owner.Stats.Armor.Total;
            float[] baseDamage = { 40f, 80f, 120f, 160f, 200f }; var damage = baseDamage[spell.CastInfo.SpellLevel - 1] + armor * 0.2f;
            float[] armorReduce = { 5f, 10f, 15f, 20f, 25f }; var reduce = armorReduce[spell.CastInfo.SpellLevel - 1] + armor * 0.05f;
            AddParticleTarget(owner, owner, "Shatter_nova", owner);

            foreach (var enemy in GetUnitsInRange(owner.Position, 375, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(owner.Team)))
            {
                var hasbuff = HasBuff((ObjAIBase)enemy, "TaricWDis");
                if (enemy is ObjAIBase)
                {
                    enemy.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    var p2 = AddParticleTarget(owner, enemy, "Shatter_tar", enemy);
                    AddBuff("TaricWDis", 4.0f, 1, spell, enemy, owner);

                    if (hasbuff == true)
                    {
                        return;
                    }
                    if (hasbuff == false)
                    {
                        enemy.Stats.Armor.FlatBonus -= reduce;
                    }

                    CreateTimer(4f, () =>
                    {
                        enemy.Stats.Armor.FlatBonus += reduce;
                        RemoveParticle(p2);
                    });
                }
            }
        }
    }
}


