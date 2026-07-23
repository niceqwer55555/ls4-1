using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using static LeagueSandbox.GameServer.API.ApiMapFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using System.Linq;
using System.Collections.Generic;
using System;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;

namespace AIScripts.Champions
{
    public class TankAI : BaseAI
    {
        private const float TANK_DETECT_RANGE = 800.0f;
        private const float TANK_COMBAT_RANGE = 1500.0f;
        private const float TANK_PULL_THRESHOLD = 0.6f;
        private const float TANK_RETREAT_HP_PERCENT = 0.2f;
        private const float TANK_INITIATE_RANGE = 400.0f;
        private float lastTauntTime = 0f;
        private float lastInitiateTime = 0f;

        public override void OnActivate(ObjAIBase owner)
        {
            base.OnActivate(owner);
            role = Role.Top;
            skillAccuracy = 0.65f;
        }

        protected override void DetectRole()
        {
            role = Role.Top;
        }

        protected override List<int> GetBuildOrder(Role r)
        {
            return new List<int> { 1001, 3047, 2049, 3068, 3143, 3102, 3083, 3401 };
        }

        protected override AttackableUnit SelectTarget()
        {
            float bestScore = float.MinValue;
            AttackableUnit bestTarget = null;
            var nearbyUnits = GetUnitsInRange(champion.Position, TANK_DETECT_RANGE, true);

            foreach (var unit in nearbyUnits)
            {
                if (!(unit is AttackableUnit u)
                    || u.IsDead
                    || u.Team == champion.Team
                    || !u.IsVisibleByTeam(champion.Team)
                    || !u.Status.HasFlag(StatusFlags.Targetable))
                {
                    continue;
                }

                if (u is BaseTurret && IsInEnemyTurretRange(u.Position))
                {
                    continue;
                }

                float score = EvaluateTankTarget(u);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = u;
                }
            }

            return bestTarget;
        }

        private float EvaluateTankTarget(AttackableUnit target)
        {
            float score = 0;

            if (target is Champion enemyChamp && enemyChamp.TargetUnit == champion)
            {
                score += 200;
            }

            float hpPercent = target.Stats.CurrentHealth / target.Stats.HealthPoints.Total;
            score += (1 - hpPercent) * 150;

            float distance = Vector2.Distance(champion.Position, target.Position);
            score += Math.Max(0, 100 - distance / 15);

            if (IsHighPriorityTarget(target))
            {
                score += 250;
            }

            var nearbyAllies = GetUnitsInRange(champion.Position, TANK_COMBAT_RANGE, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team == champion.Team && u is Champion)
                .ToList();

            if (nearbyAllies.Count > 0)
            {
                score += nearbyAllies.Count * 50;
            }

            return score;
        }

        protected override void KeepFocusingTarget()
        {
            var target = champion.TargetUnit;
            if (target == null || target.IsDead)
            {
                champion.CancelAutoAttack(false, true);
                champion.SetTargetUnit(null, true);
                return;
            }

            float distance = Vector2.Distance(champion.Position, target.Position);
            float attackRange = champion.Stats.Range.Total;

            if (target is Champion && distance > attackRange + TANK_INITIATE_RANGE)
            {
                TryInitiate(target);
            }

            if (distance > attackRange + 100f)
            {
                var path = GetPath(champion.Position, target.Position);
                if (path != null && path.Count > 1)
                {
                    champion.SetWaypoints(path);
                }
                else
                {
                    champion.SetWaypoints(new List<Vector2> { champion.Position, target.Position });
                }
                champion.UpdateMoveOrder(OrderType.MoveTo, true);
            }
            else
            {
                champion.UpdateMoveOrder(OrderType.AttackMove, true);
            }
        }

        private void TryInitiate(AttackableUnit target)
        {
            float timeSinceLastInitiate = GameTime() - lastInitiateTime;
            if (timeSinceLastInitiate < 10.0f)
            {
                return;
            }

            var nearbyAllies = GetUnitsInRange(champion.Position, TANK_COMBAT_RANGE, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team == champion.Team && u is Champion)
                .ToList();

            if (nearbyAllies.Count == 0)
            {
                return;
            }

            foreach (var spell in champion.Spells.Values)
            {
                if (spell == null || spell.CastInfo.SpellLevel <= 0 || spell.CurrentCooldown > 0)
                {
                    continue;
                }

                float range = spell.GetCurrentCastRange();
                float distance = Vector2.Distance(champion.Position, target.Position);

                if (range > 0 && distance <= range + TANK_INITIATE_RANGE)
                {
                    var targetingType = spell.SpellData.TargetingType;
                    if (targetingType == TargetingType.Target || targetingType == TargetingType.Area || targetingType == TargetingType.Cone)
                    {
                        lastInitiateTime = GameTime();
                        var castInfo = spell.CastInfo;
                        castInfo.Targets.Clear();
                        castInfo.AddTarget(target);
                        spell.Cast(champion.Position, target.Position, target);
                        return;
                    }
                }
            }
        }

        protected override void TryCastSpells()
        {
            if (lastSpellCheckTime < SPELL_CHECK_INTERVAL)
            {
                return;
            }
            lastSpellCheckTime = 0f;

            var target = champion.TargetUnit;
            if (target == null || target.IsDead)
            {
                return;
            }

            float distance = Vector2.Distance(champion.Position, target.Position);
            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;

            var spells = champion.Spells.Values
                .Where(s => s != null && s.CastInfo.SpellLevel > 0 && s.CurrentCooldown == 0)
                .OrderByDescending(s => GetTankSpellPriority(s))
                .ToList();

            foreach (var spell in spells)
            {
                float range = spell.GetCurrentCastRange();
                if (range <= 0 || distance > range)
                {
                    continue;
                }

                if (champion.Stats.ManaPoints.Total > 0)
                {
                    float manaCost = spell.SpellData.ManaCost[spell.CastInfo.SpellLevel];
                    if (champion.Stats.CurrentMana < manaCost)
                    {
                        continue;
                    }
                }

                var targetingType = spell.SpellData.TargetingType;

                if (targetingType == TargetingType.Self || targetingType == TargetingType.SelfAOE)
                {
                    if (hpPercent < TANK_PULL_THRESHOLD)
                    {
                        CastSpell(spell, champion);
                        return;
                    }
                    continue;
                }

                if (spell.CastInfo.SpellSlot == 3)
                {
                    float timeSinceLastTaunt = GameTime() - lastTauntTime;
                    if (timeSinceLastTaunt >= 10.0f)
                    {
                        lastTauntTime = GameTime();
                        CastSpell(spell, target);
                        return;
                    }
                    continue;
                }

                CastSpell(spell, target);
                return;
            }
        }

        private int GetTankSpellPriority(Spell spell)
        {
            if (spell.CastInfo.SpellSlot == 3)
            {
                return 100;
            }

            var targetingType = spell.SpellData.TargetingType;
            if (targetingType == TargetingType.Self || targetingType == TargetingType.SelfAOE)
            {
                return 90;
            }

            if (targetingType == TargetingType.Target)
            {
                return 80;
            }

            if (targetingType == TargetingType.Area || targetingType == TargetingType.Cone)
            {
                return 70;
            }

            return 50;
        }

        protected override BehaviorResult EmergencyBehavior()
        {
            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
            if (hpPercent < TANK_RETREAT_HP_PERCENT)
            {
                EmergencyHeal();
                return BehaviorResult.Success;
            }

            return base.EmergencyBehavior();
        }

        protected override void EmergencyHeal()
        {
            foreach (var spell in champion.Spells.Values)
            {
                if (spell != null && spell.CastInfo.SpellLevel > 0 && spell.CurrentCooldown == 0)
                {
                    var targetingType = spell.SpellData.TargetingType;
                    if (targetingType == TargetingType.Self || targetingType == TargetingType.SelfAOE)
                    {
                        if (champion.Stats.ManaPoints.Total == 0 ||
                            champion.Stats.CurrentMana >= spell.SpellData.ManaCost[spell.CastInfo.SpellLevel])
                        {
                            CastSpell(spell, champion);
                            return;
                        }
                    }
                }
            }

            base.EmergencyHeal();
        }

        protected override void CheckCombatState(float diff)
        {
            var nearbyUnits = GetUnitsInRange(champion.Position, TANK_COMBAT_RANGE, true);
            foreach (var unit in nearbyUnits)
            {
                if (!(unit is AttackableUnit u)
                    || u.IsDead
                    || u.Team == champion.Team
                    || !u.IsVisibleByTeam(champion.Team))
                {
                    continue;
                }

                if (u is Champion || u is BaseTurret || u is Minion)
                {
                    lastCombatTime = GameTime();
                    if (u is Champion)
                    {
                        lastKillTime = GameTime();
                    }
                    return;
                }
            }
        }

        protected override float EvaluateThreat(AttackableUnit target)
        {
            float threat = base.EvaluateThreat(target);

            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
            if (hpPercent < TANK_RETREAT_HP_PERCENT)
            {
                threat += 150;
            }

            var nearbyAllies = GetUnitsInRange(champion.Position, TANK_COMBAT_RANGE, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team == champion.Team && u is Champion)
                .ToList();

            threat -= nearbyAllies.Count * 50;

            return threat;
        }

        protected override void CheckSupportRequest()
        {
            var nearbyAllies = GetUnitsInRange(champion.Position, 2000f, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team == champion.Team && u is Champion)
                .ToList();

            foreach (var ally in nearbyAllies)
            {
                float allyHpPercent = ally.Stats.CurrentHealth / ally.Stats.HealthPoints.Total;
                if (allyHpPercent < 0.3f)
                {
                    var nearbyEnemies = GetUnitsInRange(ally.Position, 800f, true)
                        .OfType<AttackableUnit>()
                        .Where(u => !u.IsDead && u.Team != champion.Team && u is Champion)
                        .ToList();

                    if (nearbyEnemies.Count > 0)
                    {
                        float supportScore = (1 - allyHpPercent) * 100 + nearbyEnemies.Count * 60;
                        if (supportScore > 60)
                        {
                            SupportAlly(ally);
                            return;
                        }
                    }
                }
            }
        }
    }
}