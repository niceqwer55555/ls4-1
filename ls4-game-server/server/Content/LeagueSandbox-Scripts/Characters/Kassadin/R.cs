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
    public class Riftwalk : ISpellScript
    {
        private int _castCount = 0;

        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
        };

        public void OnSpellPreCast(ObjAIBase owner, Spell spell, AttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            _castCount++;

            float[] baseDamage = { 60f, 70f, 80f };
            float ap = owner.Stats.AbilityPower.Total * 0.5f;
            float damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ap;

            float[] extraDamage = { 30f, 40f, 50f };
            damage += extraDamage[spell.CastInfo.SpellLevel - 1] * (_castCount - 1);

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var distance = Vector2.Distance(owner.Position, targetPos);
            if (distance > 700)
            {
                targetPos = owner.Position + Vector2.Normalize(targetPos - owner.Position) * 700;
            }

            TeleportTo(owner, targetPos.X, targetPos.Y);

            foreach (var unit in GetUnitsInRange(owner.Position, 300f, true))
            {
                if (unit.Team != owner.Team)
                {
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }

            AddParticle(owner, owner, "Riftwalk_cas", owner.Position);

            if (_castCount > 0)
            {
                CreateTimer(1.5f, () => _castCount = 0);
            }
        }
    }
}