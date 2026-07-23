using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.StatsNS;
using System.Numerics;

namespace Spells
{
    /// <summary>
    /// Teemo R - Noxious Trap
    /// Throws a mushroom trap that explodes when enemies walk over it,
    /// dealing magic damage and slowing nearby enemies.
    /// </summary>
    public class NoxiousTrap : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnSpellPostCast(Spell spell)
        {
            var owner = spell.CastInfo.Owner as Champion;
            if (owner == null) return;

            var targetPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var ownerPos = owner.Position;

            // Validate cast range
            float castRange = spell.GetCurrentCastRange();
            var dist = Vector2.Distance(ownerPos, targetPos);
            if (dist > castRange)
            {
                var dir = Vector2.Normalize(targetPos - ownerPos);
                targetPos = ownerPos + dir * castRange;
            }

            // Spawn the mushroom trap as a minion
            var mushroom = AddMinion(owner, "TeemoMushroom", "TeemoMushroom", targetPos, owner.Team, isVisible: true);

            if (mushroom != null)
            {
                // Set trap duration: 10 minutes
                CreateTimer(600f, () =>
                {
                    if (!mushroom.IsDead)
                    {
                        mushroom.TakeDamage(owner, 10000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
                    }
                });

                // Add particle for the trap placement
                AddParticle(owner, null, "NoxiousTrap_cas.troy", targetPos);
            }
        }
    }
}
