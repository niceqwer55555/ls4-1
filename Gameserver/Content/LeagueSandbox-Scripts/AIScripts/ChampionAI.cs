using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
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
    public class ChampionAI : IAIScript
    {
        public AIScriptMetaData AIScriptMetaData { get; set; } = new AIScriptMetaData();
        Champion champion;
        List<Vector2> laneWaypoints;
        int currentWaypoint = 0;
        bool itemsBought = false;
        bool retreating = false;
        bool skillsLeveled = false;
        float lastSpellCheckTime = 0f;
        float stuckCheckTimer = 0f;
        Vector2 lastPosition;
        int stuckCounter = 0;
        int laneOffset = 0;
        Random random = new Random();

        const float DETECT_RANGE = 700.0f;
        const float WAYPOINT_TOLERANCE = 300.0f;
        const float RETREAT_HP_PERCENT = 0.25f;
        const float SPELL_CHECK_INTERVAL = 1.5f;
        const float STUCK_CHECK_INTERVAL = 2.0f;
        const float STUCK_DISTANCE = 50f;
        const float TURRET_DANGER_RANGE = 950.0f;
        const float TURRET_SAFE_RANGE = 1100.0f;

        public void OnActivate(ObjAIBase owner)
        {
            champion = owner as Champion;
            lastPosition = champion.Position;
            AssignLaneWaypoints();
        }

        void AssignLaneWaypoints()
        {
            string name = champion.Name;
            bool isBlue = champion.Team == TeamId.TEAM_BLUE;

            laneWaypoints = new List<Vector2>();
            Vector2 basePos = isBlue ? new Vector2(0, 1000) : new Vector2(13300, 14600);

            bool isTopLaner = name.Contains("Top") || name.Contains("Jungle");
            bool isMidLaner = name.Contains("Mid");
            bool isBotLaner = name.Contains("ADC") || name.Contains("Support");

            if (isTopLaner)
            {
                laneOffset = name.Contains("Jungle") ? 1 : 0;

                if (isBlue)
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(800, 6500));
                    laneWaypoints.Add(new Vector2(575, 10220));
                    laneWaypoints.Add(new Vector2(2000, 12000));
                    laneWaypoints.Add(new Vector2(3912, 13655));
                }
                else
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(12500, 8000));
                    laneWaypoints.Add(new Vector2(9700, 12500));
                    laneWaypoints.Add(new Vector2(5900, 12500));
                    laneWaypoints.Add(new Vector2(3912, 13655));
                }
            }
            else if (isMidLaner)
            {
                if (isBlue)
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(3000, 3500));
                    laneWaypoints.Add(new Vector2(5448, 6169));
                    laneWaypoints.Add(new Vector2(7000, 7000));
                    laneWaypoints.Add(new Vector2(8549, 8289));
                }
                else
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(11000, 10000));
                    laneWaypoints.Add(new Vector2(8549, 8289));
                    laneWaypoints.Add(new Vector2(7000, 7000));
                    laneWaypoints.Add(new Vector2(5448, 6169));
                }
            }
            else if (isBotLaner)
            {
                laneOffset = name.Contains("Support") ? 1 : 0;

                if (isBlue)
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(6500, 1263));
                    laneWaypoints.Add(new Vector2(10098, 809));
                    laneWaypoints.Add(new Vector2(11500, 2500));
                    laneWaypoints.Add(new Vector2(13460, 4284));
                }
                else
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(7500, 13500));
                    laneWaypoints.Add(new Vector2(4284, 13460));
                    laneWaypoints.Add(new Vector2(2500, 11500));
                    laneWaypoints.Add(new Vector2(809, 10098));
                }
            }
            else
            {
                if (isBlue)
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(3000, 3500));
                    laneWaypoints.Add(new Vector2(5448, 6169));
                }
                else
                {
                    laneWaypoints.Add(basePos);
                    laneWaypoints.Add(new Vector2(11000, 10000));
                    laneWaypoints.Add(new Vector2(8549, 8289));
                }
            }

            currentWaypoint = 1;
        }

        Vector2 ApplyLaneOffset(Vector2 target)
        {
            if (laneOffset == 0)
            {
                return target;
            }

            Vector2 toTarget = target - champion.Position;
            if (toTarget.LengthSquared() < 1f)
            {
                return target;
            }

            Vector2 perpendicular = new Vector2(-toTarget.Y, toTarget.X);
            perpendicular = Vector2.Normalize(perpendicular);
            return target + perpendicular * 150f;
        }

        public void OnUpdate(float diff)
        {
            if (champion == null || champion.IsDead)
            {
                return;
            }

            if (!itemsBought)
            {
                BuyStartingItems();
            }

            if (!skillsLeveled && champion.Stats.Level >= 1)
            {
                LevelUpStartingSkill();
            }

            if (champion.IsAIPaused())
            {
                return;
            }

            lastSpellCheckTime += diff / 1000f;
            stuckCheckTimer += diff / 1000f;

            UpdateBehavior(diff);
        }

        void BuyStartingItems()
        {
            itemsBought = true;
            champion.AddGold(null, 1000, false);

            string name = champion.Name;
            var items = GetRecommendedItems(name);
            foreach (var itemId in items)
            {
                champion.Shop.HandleItemBuyRequest(itemId);
            }
        }

        List<int> GetRecommendedItems(string championName)
        {
            if (championName.Contains("Top"))
            {
                return new List<int> { 1055, 2003, 3340 };
            }
            if (championName.Contains("Jungle"))
            {
                return new List<int> { 1039, 2003, 3340 };
            }
            if (championName.Contains("Mid"))
            {
                return new List<int> { 1056, 2003, 3340 };
            }
            if (championName.Contains("ADC"))
            {
                return new List<int> { 1055, 2003, 3340 };
            }
            if (championName.Contains("Support"))
            {
                return new List<int> { 1054, 2003, 3340 };
            }
            return new List<int> { 1055, 2003 };
        }

        void LevelUpStartingSkill()
        {
            skillsLeveled = true;
            byte skillSlot = 0;
            string name = champion.Name;

            if (name.Contains("Top"))
            {
                skillSlot = 0;
            }
            else if (name.Contains("Mid"))
            {
                skillSlot = 0;
            }
            else if (name.Contains("ADC"))
            {
                skillSlot = 1;
            }
            else if (name.Contains("Support"))
            {
                skillSlot = 2;
            }
            else if (name.Contains("Jungle"))
            {
                skillSlot = 2;
            }
            else
            {
                skillSlot = 0;
            }

            if (champion.Spells.ContainsKey(skillSlot))
            {
                champion.LevelUpSpell(skillSlot);
            }
        }

        void UpdateBehavior(float diff)
        {
            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;

            if (hpPercent < RETREAT_HP_PERCENT)
            {
                retreating = true;
                RetreatToBase();
                return;
            }

            if (retreating && hpPercent > 0.9f)
            {
                retreating = false;
                currentWaypoint = 1;
            }

            if (retreating)
            {
                RetreatToBase();
                return;
            }

            if (IsInEnemyTurretRange(champion.Position))
            {
                RetreatFromTurret();
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

        bool IsInEnemyTurretRange(Vector2 position)
        {
            var nearbyUnits = GetUnitsInRange(position, TURRET_DANGER_RANGE, true);
            foreach (var unit in nearbyUnits)
            {
                if (unit is BaseTurret turret
                    && !turret.IsDead
                    && turret.Team != champion.Team)
                {
                    float turretRange = turret.Stats.Range.Total;
                    if (Vector2.Distance(position, turret.Position) < turretRange + 50f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool IsTargetInEnemyTurretRange(Vector2 target)
        {
            return IsInEnemyTurretRange(target);
        }

        void RetreatFromTurret()
        {
            BaseTurret nearestTurret = null;
            float nearestDist = float.MaxValue;

            var nearbyUnits = GetUnitsInRange(champion.Position, TURRET_SAFE_RANGE, true);
            foreach (var unit in nearbyUnits)
            {
                if (unit is BaseTurret turret
                    && !turret.IsDead
                    && turret.Team != champion.Team)
                {
                    float dist = Vector2.Distance(champion.Position, turret.Position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestTurret = turret;
                    }
                }
            }

            if (nearestTurret == null)
            {
                return;
            }

            Vector2 away = champion.Position - nearestTurret.Position;
            if (away.LengthSquared() < 1f)
            {
                away = champion.Team == TeamId.TEAM_BLUE
                    ? new Vector2(-1, -1)
                    : new Vector2(1, 1);
            }
            away = Vector2.Normalize(away);
            Vector2 safePos = champion.Position + away * 300f;

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

        bool ScanForTargets()
        {
            if (champion.TargetUnit != null && !champion.TargetUnit.IsDead)
            {
                if (IsTargetInEnemyTurretRange(champion.TargetUnit.Position))
                {
                    champion.CancelAutoAttack(false, true);
                    champion.SetTargetUnit(null, true);
                    return false;
                }
                return true;
            }

            AttackableUnit nextTarget = null;
            var nextTargetPriority = 14;
            var nearbyUnits = GetUnitsInRange(champion.Position, DETECT_RANGE, true);

            foreach (var unit in nearbyUnits.OrderBy(x => Vector2.DistanceSquared(champion.Position, x.Position)))
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

                var priority = (int)champion.ClassifyTarget(u);
                if (priority < nextTargetPriority)
                {
                    nextTarget = u;
                    nextTargetPriority = priority;
                }
            }

            if (nextTarget != null)
            {
                champion.SetTargetUnit(nextTarget, true);
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
        }

        void TryCastSpells()
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

            for (byte i = 0; i < 4; i++)
            {
                if (!champion.Spells.ContainsKey(i))
                {
                    continue;
                }

                var spell = champion.Spells[i];
                if (spell == null)
                {
                    continue;
                }

                if (spell.CastInfo.SpellLevel <= 0)
                {
                    continue;
                }

                if (spell.CurrentCooldown > 0)
                {
                    continue;
                }

                float range = spell.GetCurrentCastRange();
                if (range <= 0 || distance > range)
                {
                    continue;
                }

                var targetingType = spell.SpellData.TargetingType;
                if (targetingType == TargetingType.Self || targetingType == TargetingType.SelfAOE)
                {
                    continue;
                }

                CastSpell(spell, target);
                return;
            }
        }

        void CastSpell(Spell spell, AttackableUnit target)
        {
            var castInfo = spell.CastInfo;
            castInfo.Targets.Clear();
            castInfo.AddTarget(target);

            spell.Cast(champion.Position, target.Position, target);
        }

        void MoveAlongLane(float diff)
        {
            if (currentWaypoint >= laneWaypoints.Count)
            {
                currentWaypoint = laneWaypoints.Count - 1;
            }

            Vector2 target = laneWaypoints[currentWaypoint];
            target = ApplyLaneOffset(target);
            float distance = Vector2.Distance(champion.Position, target);

            if (distance < WAYPOINT_TOLERANCE && currentWaypoint < laneWaypoints.Count - 1)
            {
                currentWaypoint++;
                target = ApplyLaneOffset(laneWaypoints[currentWaypoint]);
                stuckCounter = 0;
            }

            if (IsTargetInEnemyTurretRange(target))
            {
                Vector2 retreatDir = champion.Team == TeamId.TEAM_BLUE
                    ? new Vector2(-1, -1)
                    : new Vector2(1, 1);
                retreatDir = Vector2.Normalize(retreatDir);
                Vector2 safeTarget = target + retreatDir * 400f;

                var path = GetPath(champion.Position, safeTarget);
                if (path != null && path.Count > 1)
                {
                    champion.SetWaypoints(path);
                }
                else
                {
                    champion.SetWaypoints(new List<Vector2> { champion.Position, safeTarget });
                }
                champion.UpdateMoveOrder(OrderType.MoveTo, true);
                return;
            }

            CheckStuck(diff);

            var movePath = GetPath(champion.Position, target);
            if (movePath != null && movePath.Count > 1)
            {
                champion.SetWaypoints(movePath);
            }
            else
            {
                champion.SetWaypoints(new List<Vector2> { champion.Position, target });
            }
            champion.UpdateMoveOrder(OrderType.MoveTo, true);
        }

        void CheckStuck(float diff)
        {
            if (stuckCheckTimer < STUCK_CHECK_INTERVAL)
            {
                return;
            }
            stuckCheckTimer = 0f;

            float moveDist = Vector2.Distance(champion.Position, lastPosition);
            if (moveDist < STUCK_DISTANCE)
            {
                stuckCounter++;
            }
            else
            {
                stuckCounter = 0;
            }
            lastPosition = champion.Position;

            if (stuckCounter >= 3)
            {
                stuckCounter = 0;
                Vector2 randDir = new Vector2(
                    (float)(random.NextDouble() * 2 - 1),
                    (float)(random.NextDouble() * 2 - 1)
                );
                randDir = Vector2.Normalize(randDir);
                Vector2 bypassPos = champion.Position + randDir * 200f;

                var bypassPath = GetPath(champion.Position, bypassPos);
                if (bypassPath != null && bypassPath.Count > 1)
                {
                    champion.SetWaypoints(bypassPath);
                }
                else
                {
                    champion.SetWaypoints(new List<Vector2> { champion.Position, bypassPos });
                }
                champion.UpdateMoveOrder(OrderType.MoveTo, true);

                if (currentWaypoint < laneWaypoints.Count - 1)
                {
                    currentWaypoint++;
                }
            }
        }

        void RetreatToBase()
        {
            Vector2 basePos = champion.Team == TeamId.TEAM_BLUE
                ? new Vector2(0, 1000)
                : new Vector2(13300, 14600);

            var path = GetPath(champion.Position, basePos);
            if (path != null && path.Count > 1)
            {
                champion.SetWaypoints(path);
            }
            else
            {
                champion.SetWaypoints(new List<Vector2> { champion.Position, basePos });
            }
            champion.UpdateMoveOrder(OrderType.MoveTo, true);
            champion.CancelAutoAttack(false, true);
            champion.SetTargetUnit(null, true);
        }
    }
}
