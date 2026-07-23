using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace CharScripts
{
    /// <summary>
    /// MasterYi - The Wuju Bladesman
    /// Passive: Double Strike - every 3rd attack triggers a double strike.
    /// </summary>
    public class CharScriptMasterYi : ICharScript
    {
        private ObjAIBase _owner;
        private Spell _spell;

        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            _owner = owner;
            _spell = spell;

            // Add passive tracking buff - tracks attack count for Double Strike
            AddBuff("MasterYiPassive", 1f, 1, spell, owner, owner, true);

            // Listen for auto-attacks to stack passive
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, false);
        }

        /// <summary>
        /// Stack passive on each auto-attack. At 3 stacks, trigger Double Strike.
        /// </summary>
        public void OnLaunchAttack(Spell spell)
        {
            if (_owner.HasBuff("MasterYiDoubleStrike"))
            {
                // Double Strike attack in progress, reset passive after
                return;
            }

            // Add a stack of MasterYiPassive
            AddBuff("MasterYiPassive", 1f, 1, spell, _owner, _owner);
        }
    }
}
