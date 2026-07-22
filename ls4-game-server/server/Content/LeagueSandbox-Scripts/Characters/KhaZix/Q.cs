using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using System.Linq;
using System.Numerics;

namespace Spells
{
    public class KhazixQ : ISpellScript
    {
        float Damage;
        private ObjAIBase Khazix;
        private AttackableUnit Target;
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            IsDamagingSpell = true,
            TriggersSpellCasts = true
        };
        public void OnSpellPostCast(Spell spell)
        {
            Khazix = spell.CastInfo.Owner as Champion;
            Target = Khazix.Spells[0].CastInfo.Targets[0].Unit;

            float[] baseDamage = { 80f, 100f, 120f, 140f, 160f };
            float ad = Khazix.Stats.AttackDamage.Total;
            float ap = Khazix.Stats.AbilityPower.Total * 0.5f;
            Damage = baseDamage[spell.CastInfo.SpellLevel - 1] + ad + ap;

            Target.TakeDamage(Khazix, Damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            AddParticleTarget(Khazix, Target, "Khazix_Base_Q_MultiEnemy_Tar.troy", Target, 1f, 1f);
        }
    }
}