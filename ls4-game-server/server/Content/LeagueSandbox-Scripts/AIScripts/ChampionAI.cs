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

namespace AIScripts
{
    public class ChampionAI : BaseAI
    {
        enum BotState { Idle, Laning, Combat, Chase, Flee, Recall, Shop, Roam }

        BotState currentState = BotState.Laning;
        AttackableUnit currentTarget;
        float lastDecisionTime = 0f;

        const float DECISION_INTERVAL = 0.5f;
        const float SURROUND_FLEE_RANGE = 650f;
        const float SURROUND_FLEE_HP_PERCENT = 0.45f;
        const int SURROUND_FLEE_ENEMY_COUNT = 2;

        public override void OnActivate(ObjAIBase owner)
        {
            base.OnActivate(owner);
            currentState = BotState.Laning;
        }

        public override void OnUpdate(float diff)
        {
            if (champion == null || champion.IsDead)
            {
                return;
            }

            AutoLevelSkills();
            TryBuyItems();

            if (champion.IsAIPaused())
            {
                return;
            }

            if (IsInEnemyTurretRange(champion.Position) && !IsAtBase())
            {
                currentState = BotState.Flee;
                HandleFleeState();
                return;
            }

            lastSpellCheckTime += diff / 1000f;
            stuckCheckTimer += diff / 1000f;
            lastDecisionTime += diff / 1000f;

            UpdateState(diff);
            RunStateBehavior(diff);
        }

        // Uses base AI auto-level and item-buy behavior unless bot-specific adjustments are required.

        void UpdateState(float diff)
        {
            if (lastDecisionTime < DECISION_INTERVAL)
            {
                return;
            }

            lastDecisionTime = 0f;

            float hpPercent = GetHealthPercent();
            float manaPercent = GetManaPercent();
            bool lowHealth = hpPercent < RETREAT_HP_PERCENT || manaPercent < RETREAT_MANA_PERCENT;
            bool inCombat = IsInCombat();
            bool underEnemyTurret = IsInEnemyTurretRange(champion.Position);
            bool hasTarget = SelectBestTarget() != null;
            bool shouldRecall = ShouldRecallToShop(hpPercent, manaPercent, hasTarget);
            bool shouldShop = IsAtBase() && buildIndex < buildOrder.Count;
            bool shouldFlee = ShouldFleeFromThreat(hpPercent);

            if (underEnemyTurret && !IsAtBase())
            {
                currentState = BotState.Flee;
                return;
            }

            if (shouldFlee && !IsAtBase())
            {
                currentState = BotState.Flee;
                return;
            }

            if (lowHealth && !IsAtBase())
            {
                currentState = BotState.Flee;
                return;
            }

            if (shouldRecall)
            {
                currentState = BotState.Recall;
                return;
            }

            if (shouldShop)
            {
                currentState = BotState.Shop;
                return;
            }

            if (hasTarget || inCombat)
            {
                currentState = BotState.Combat;
                return;
            }

            currentState = BotState.Laning;
        }

        void RunStateBehavior(float diff)
        {
            switch (currentState)
            {
                case BotState.Flee:
                    HandleFleeState();
                    break;
                case BotState.Recall:
                    HandleRecallState();
                    break;
                case BotState.Shop:
                    HandleShopState();
                    break;
                case BotState.Combat:
                    HandleCombatState();
                    break;
                case BotState.Laning:
                default:
                    HandleLaningState(diff);
                    break;
            }
        }

        void HandleFleeState()
        {
            float hpPercent = GetHealthPercent();
            if (IsAtBase())
            {
                currentState = BotState.Laning;
                retreating = false;
                return;
            }

            if (hpPercent > 0.6f && !IsInEnemyTurretRange(champion.Position))
            {
                currentState = BotState.Laning;
                return;
            }

            if (champion.Stats.Gold >= RECALL_GOLD_THRESHOLD && !IsAtBase())
            {
                champion.Recall();
            }
            else
            {
                RetreatToBase();
            }
        }

        void HandleRecallState()
        {
            if (IsAtBase())
            {
                currentState = BotState.Shop;
                return;
            }

            champion.Recall();
        }

        void HandleShopState()
        {
            TryBuyItems();
            float hpPercent = GetHealthPercent();
            float manaPercent = GetManaPercent();
            bool canAffordNextItem = buildIndex >= buildOrder.Count;
            if (!canAffordNextItem)
            {
                var itemData = GetItemData(buildOrder[buildIndex]);
                canAffordNextItem = itemData != null && champion.Stats.Gold >= itemData.TotalPrice;
            }
            if (hpPercent > 0.95f && manaPercent > 0.95f)
            {
                currentState = BotState.Laning;
            }
        }

        void HandleCombatState()
        {
            if (ShouldFleeFromThreat(GetHealthPercent()))
            {
                currentState = BotState.Flee;
                HandleFleeState();
                return;
            }

            if (IsInEnemyTurretRange(champion.Position) && !IsAtBase())
            {
                currentState = BotState.Flee;
                HandleFleeState();
                return;
            }

            if (!ScanForTargets())
            {
                if (IsInEnemyTurretRange(champion.Position) && !IsAtBase())
                {
                    currentState = BotState.Flee;
                    HandleFleeState();
                    return;
                }

                currentState = BotState.Laning;
                return;
            }

            KeepFocusingTarget();
            TryCastSpells();
        }

        void HandleLaningState(float diff)
        {
            if (ShouldFleeFromThreat(GetHealthPercent()))
            {
                currentState = BotState.Flee;
                HandleFleeState();
                return;
            }

            if (IsInEnemyTurretRange(champion.Position) && !IsAtBase())
            {
                currentState = BotState.Flee;
                HandleFleeState();
                return;
            }

            if (ScanForTargets())
            {
                KeepFocusingTarget();
                TryCastSpells();
                return;
            }

            MoveAlongLane(diff);
        }

        bool ShouldFleeFromThreat(float hpPercent)
        {
            if (IsAtBase())
            {
                return false;
            }

            int nearbyEnemies = CountNearbyEnemyChampions(champion.Position, SURROUND_FLEE_RANGE);
            bool lowHealth = hpPercent < RETREAT_HP_PERCENT;
            bool lowHealthUnderPressure = hpPercent < SURROUND_FLEE_HP_PERCENT && nearbyEnemies >= SURROUND_FLEE_ENEMY_COUNT;
            bool beingSurrounded = nearbyEnemies >= SURROUND_FLEE_ENEMY_COUNT && hpPercent < 0.6f;
            bool inCombatAndOutnumbered = nearbyEnemies >= 2 && IsInCombat() && hpPercent < 0.7f;

            return lowHealth || lowHealthUnderPressure || beingSurrounded || inCombatAndOutnumbered;
        }

        int CountNearbyEnemyChampions(Vector2 position, float range)
        {
            int count = 0;
            var nearbyUnits = GetUnitsInRange(position, range, true);
            foreach (var unit in nearbyUnits)
            {
                if (unit is Champion enemyChampion
                    && !enemyChampion.IsDead
                    && enemyChampion.Team != champion.Team
                    && enemyChampion.IsVisibleByTeam(champion.Team))
                {
                    count++;
                }
            }

            return count;
        }

        float GetHealthPercent()
        {
            return champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
        }

        float GetManaPercent()
        {
            if (champion.Stats.ManaPoints.Total <= 0)
            {
                return 1f;
            }
            return champion.Stats.CurrentMana / champion.Stats.ManaPoints.Total;
        }

        bool ShouldRecallToShop(float hpPercent, float manaPercent, bool hasTarget)
        {
            if (buildIndex >= buildOrder.Count)
            {
                return false;
            }

            if (IsAtBase())
            {
                return false;
            }

            if (hpPercent < RETREAT_HP_PERCENT || manaPercent < RETREAT_MANA_PERCENT)
            {
                return true;
            }

            return champion.Stats.Gold >= RECALL_GOLD_THRESHOLD && !hasTarget;
        }

        AttackableUnit SelectBestTarget()
        {
            if (champion.TargetUnit != null && !champion.TargetUnit.IsDead && champion.TargetUnit.Team != champion.Team)
            {
                currentTarget = champion.TargetUnit as AttackableUnit;
                return currentTarget;
            }

            AttackableUnit bestTarget = null;
            float bestScore = float.MaxValue;
            var nearbyUnits = GetUnitsInRange(champion.Position, DETECT_RANGE, true);

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

                if (IsTargetInEnemyTurretRange(u.Position))
                {
                    continue;
                }

                int priority = (int)champion.ClassifyTarget(u);
                float distance = Vector2.Distance(champion.Position, u.Position);
                float hpPercent = u.Stats.CurrentHealth / u.Stats.HealthPoints.Total;
                float score = priority * 100f + distance * 0.1f - hpPercent * 50f;

                if (score < bestScore)
                {
                    bestTarget = u;
                    bestScore = score;
                }
            }

            if (bestTarget != null)
            {
                currentTarget = bestTarget;
                champion.SetTargetUnit(bestTarget, true);
                return bestTarget;
            }

            currentTarget = null;
            return null;
        }

        bool ScanForTargets()
        {
            if (champion.TargetUnit != null && !champion.TargetUnit.IsDead)
            {
                if (IsTargetInEnemyTurretRange(champion.TargetUnit.Position))
                {
                    champion.CancelAutoAttack(false, true);
                    champion.SetTargetUnit(null, true);
                    currentTarget = null;
                    return false;
                }

                currentTarget = champion.TargetUnit as AttackableUnit;
                return true;
            }

            var bestTarget = SelectBestTarget();
            if (bestTarget != null)
            {
                float distance = Vector2.Distance(champion.Position, bestTarget.Position);
                float attackRange = champion.Stats.Range.Total;

                if (distance > attackRange + 100f)
                {
                    var path = GetPath(champion.Position, bestTarget.Position);
                    if (path != null && path.Count > 1)
                    {
                        champion.SetWaypoints(path);
                    }
                    else
                    {
                        champion.SetWaypoints(new List<Vector2> { champion.Position, bestTarget.Position });
                    }
                    champion.UpdateMoveOrder(OrderType.MoveTo, true);
                }
                else
                {
                    champion.UpdateMoveOrder(OrderType.AttackMove, true);
                }
                return true;
            }

            return false;
        }

        void KeepFocusingTarget()
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

        bool IsInCombat()
        {
            float timeSinceLastCombat = GameTime() - lastCombatTime;
            float timeSinceLastKill = GameTime() - lastKillTime;

            if (timeSinceLastCombat < COMBAT_TIMEOUT)
            {
                return true;
            }

            if (timeSinceLastKill > 0 && timeSinceLastKill < POST_COMBAT_RECALL_DELAY)
            {
                return true;
            }

            return false;
        }

        void CheckCombatState(float diff)
        {
            var nearbyUnits = GetUnitsInRange(champion.Position, COMBAT_RANGE, true);
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

        bool IsInEnemyTurretRange(Vector2 position)
        {
            return IsUnderEnemyTurret(position, out _);
        }

        bool IsTargetInEnemyTurretRange(Vector2 target)
        {
            return IsUnderEnemyTurret(target, out _);
        }

        BaseTurret GetNearestEnemyTurret(Vector2 position)
        {
            BaseTurret nearestTurret = null;
            float nearestDist = float.MaxValue;

            var nearbyUnits = GetUnitsInRange(position, TURRET_SAFE_RANGE, true);
            foreach (var unit in nearbyUnits)
            {
                if (unit is BaseTurret turret
                    && !turret.IsDead
                    && turret.Team != champion.Team)
                {
                    float dist = Vector2.Distance(position, turret.Position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestTurret = turret;
                    }
                }
            }

            return nearestTurret;
        }

        bool IsUnderEnemyTurret(Vector2 position, out BaseTurret nearestTurret, float margin = 50f)
        {
            nearestTurret = GetNearestEnemyTurret(position);
            if (nearestTurret == null)
            {
                return false;
            }

            float turretRange = nearestTurret.Stats.Range.Total;
            return Vector2.Distance(position, nearestTurret.Position) < turretRange + margin;
        }

        Vector2 GetSafePositionFromTurret(BaseTurret turret, Vector2 fromPosition, float extraDistance = 250f)
        {
            Vector2 away = fromPosition - turret.Position;
            if (away.LengthSquared() < 1f)
            {
                away = champion.Team == TeamId.TEAM_BLUE
                    ? new Vector2(-1, -1)
                    : new Vector2(1, 1);
            }
            away = Vector2.Normalize(away);

            float turretRange = turret.Stats.Range.Total;
            float safeDistance = turretRange + extraDistance;
            return turret.Position + away * safeDistance;
        }

        void RetreatFromTurret()
        {
            var nearestTurret = GetNearestEnemyTurret(champion.Position);
            if (nearestTurret == null)
            {
                return;
            }

            Vector2 safePos = GetSafePositionFromTurret(nearestTurret, champion.Position, 300f);

            var path = GetPath(champion.Position, safePos);
            if (path != null && path.Count > 1)
            {
                champion.SetWaypoints(path);
            }
            else
            {
                champion.SetWaypoints(new List<Vector2> { champion.Position, safePos });
            }
            champion.UpdateMoveOrder(OrderType.MoveTo, true);
            champion.CancelAutoAttack(false, true);
            champion.SetTargetUnit(null, true);
        }

        // Custom lane movement and support logic override base movement behavior when needed.
    }
}