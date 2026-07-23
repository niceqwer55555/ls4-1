using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using GameServerLib.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace CharScripts
{
    /// <summary>
    /// Teemo - The Swift Scout
    /// Manages W passive move speed buff and level-up range bonus.
    /// </summary>
    public class CharScriptTeemo : ICharScript
    {
        Spell Passive;
        ObjAIBase Teemo;
        public StatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(ObjAIBase owner, Spell spell = null)
        {
            Passive = spell;
            Teemo = owner as Champion;
            ApiEventManager.OnKill.AddListener(this, Teemo, OnKill, false);
            ApiEventManager.OnLevelUp.AddListener(this, Teemo, OnLevelUp, false);
        }

        public void OnKill(DeathData deathData)
        {
            // Kill resets W cooldown
            Teemo.Spells[1].SetCooldown(0f, true);
        }

        public void OnLevelUp(AttackableUnit target)
        {
            StatsModifier.Range.FlatBonus = 7.5f;
            Teemo.AddStatModifier(StatsModifier);
        }
    }
}
