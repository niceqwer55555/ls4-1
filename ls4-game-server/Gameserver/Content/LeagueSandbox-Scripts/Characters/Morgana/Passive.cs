using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerLib.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class SoulSiphon : ISpellScript
    {
        public SpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = false
        };

        public void OnActivate(ObjAIBase owner, Spell spell)
        {
            ApiEventManager.OnHitUnit.AddListener(this, owner, OnHitUnit, false);
        }

        private void OnHitUnit(DamageData damageData)
        {
            var owner = damageData.Attacker;
            var target = damageData.Target;
            var damage = damageData.Damage;

            if (damageData.DamageType == DamageType.DAMAGE_TYPE_MAGICAL)
            {
                var healAmount = damage * 0.2f;
                owner.Stats.CurrentHealth += healAmount;
                AddParticle(owner, owner, "Global_Heal.troy", owner.Position);
            }
        }

        public void OnDeactivate(ObjAIBase owner, Spell spell)
        {
            ApiEventManager.OnHitUnit.RemoveListener(this);
        }
    }
}