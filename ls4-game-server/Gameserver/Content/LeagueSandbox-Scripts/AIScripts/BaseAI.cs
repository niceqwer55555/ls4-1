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
    public abstract class BaseAI : IAIScript
    {
        public AIScriptMetaData AIScriptMetaData { get; set; } = new AIScriptMetaData();

        protected Champion champion;
        protected List<Vector2> laneWaypoints;
        protected int currentWaypoint = 0;
        protected bool retreating = false;
        protected float lastSpellCheckTime = 0f;
        protected float stuckCheckTimer = 0f;
        protected Vector2 lastPosition;
        protected int stuckCounter = 0;
        protected int laneOffset = 0;
        protected Random random = new Random();

        protected enum Role { Top, Jungle, Mid, ADC, Support, Unknown }
        protected Role role = Role.Unknown;
        protected List<int> buildOrder;
        protected int buildIndex = 0;

        protected const float DETECT_RANGE = 900.0f;
        protected const float WAYPOINT_TOLERANCE = 300.0f;
        protected const float RETREAT_HP_PERCENT = 0.25f;
        protected const float RETREAT_MANA_PERCENT = 0.15f;
        protected const float SPELL_CHECK_INTERVAL = 1.5f;
        protected const float STUCK_CHECK_INTERVAL = 2.0f;
        protected const float STUCK_DISTANCE = 50f;
        protected const float TURRET_DANGER_RANGE = 950.0f;
        protected const float TURRET_SAFE_RANGE = 1100.0f;
        protected const float BASE_RADIUS = 1200.0f;
        protected const float RECALL_GOLD_THRESHOLD = 600f;
        protected const float COMBAT_RANGE = 1800.0f;
        protected const float COMBAT_TIMEOUT = 20.0f;
        protected const float POST_COMBAT_RECALL_DELAY = 15.0f;
        protected float lastCombatTime = 0f;
        protected float lastKillTime = 0f;

        protected float difficulty = 0.7f;
        protected float reactionTime = 0.5f;
        protected float skillAccuracy = 0.7f;
        protected float csAccuracy = 0.85f;

        protected enum AIState
        {
            IDLE,
            LANING,
            COMBAT,
            CHASE,
            FLEEING,
            RECALLED,
            SHOPPING,
            RETURNING,
            SUPPORTING
        }
        protected AIState currentState = AIState.IDLE;

        protected static readonly Dictionary<Role, byte[]> skillPriority = new Dictionary<Role, byte[]>
        {
            { Role.Top,     new byte[]{0,2,1,0,0,3,0,2,2,2,3,2,1,1,1,3,1,1} },
            { Role.Jungle,  new byte[]{2,0,1,2,2,3,2,0,0,0,3,0,1,1,1,3,1,1} },
            { Role.Mid,     new byte[]{0,2,1,0,0,3,0,1,1,1,3,1,2,2,2,3,2,2} },
            { Role.ADC,     new byte[]{1,0,2,1,1,3,1,0,0,0,3,0,2,2,2,3,2,2} },
            { Role.Support, new byte[]{2,0,1,2,2,3,2,1,1,1,3,1,0,0,0,3,0,0} },
        };

        public virtual void OnActivate(ObjAIBase owner)
        {
            champion = owner as Champion;
            lastPosition = champion.Position;
            DetectRole();
            buildOrder = GetBuildOrder(role);
            AssignLaneWaypoints();
            InitializeDifficulty();
        }

        protected virtual void InitializeDifficulty()
        {
            difficulty = 0.7f;
            reactionTime = 0.8f;
            skillAccuracy = 0.7f;
            csAccuracy = 0.85f;
        }

        protected virtual void DetectRole()
        {
            string name = champion.Name;
            if (name.Contains("Top"))
            {
                role = Role.Top;
            }
            else if (name.Contains("Jungle"))
            {
                role = Role.Jungle;
            }
            else if (name.Contains("Mid"))
            {
                role = Role.Mid;
            }
            else if (name.Contains("ADC"))
            {
                role = Role.ADC;
            }
            else if (name.Contains("Support"))
            {
                role = Role.Support;
            }
            else
            {
                role = Role.Unknown;
            }
        }

        protected virtual List<int> GetBuildOrder(Role r)
        {
            string championName = champion.Name;
            if (championName.Contains("Top") || championName.Contains("Jungle") ||
                championName.Contains("Mid") || championName.Contains("ADC") ||
                championName.Contains("Support"))
            {
                championName = championName.Replace("Top", "").Replace("Jungle", "")
                                           .Replace("Mid", "").Replace("ADC", "")
                                           .Replace("Support", "");
            }

            List<int> adcBuild = new List<int> { 1001, 3006, 1036, 3031, 3046, 3072, 3035, 3074 };
            List<int> apBuild = new List<int> { 1001, 3020, 1058, 3089, 3135, 3157, 3041, 3116 };
            List<int> tankBuild = new List<int> { 1001, 3047, 2049, 3068, 3143, 3102, 3083, 3401 };
            List<int> bruiserBuild = new List<int> { 1001, 3006, 1036, 3066, 3046, 3072, 3035, 3156 };
            List<int> assassinBuild = new List<int> { 1001, 3006, 1036, 3047, 3121, 3072, 3035, 3153 };

            string lowerName = championName.ToLower();
            if (IsAdcChampion(lowerName))
            {
                return adcBuild;
            }
            else if (IsApChampion(lowerName))
            {
                return apBuild;
            }
            else if (IsTankChampion(lowerName))
            {
                return tankBuild;
            }
            else if (IsBruiserChampion(lowerName))
            {
                return bruiserBuild;
            }
            else if (IsAssassinChampion(lowerName))
            {
                return assassinBuild;
            }

            switch (r)
            {
                case Role.Mid:
                    return apBuild;
                case Role.Support:
                    return tankBuild;
                case Role.ADC:
                    return adcBuild;
                case Role.Jungle:
                    return bruiserBuild;
                default:
                    return bruiserBuild;
            }
        }

        protected virtual bool IsAdcChampion(string name)
        {
            return name.Contains("ashe") || name.Contains("caitlyn") || name.Contains("draven") ||
                   name.Contains("ezreal") || name.Contains("varus") || name.Contains("vayne") ||
                   name.Contains("corki") || name.Contains("jinx") || name.Contains("tristana") ||
                   name.Contains("sivir") || name.Contains("kalista") || name.Contains("lucian") ||
                   name.Contains("kogmaw") || name.Contains("missfortune") || name.Contains("quinn");
        }

        protected virtual bool IsApChampion(string name)
        {
            return name.Contains("annie") || name.Contains("veigar") || name.Contains("ziggs") ||
                   name.Contains("xerath") || name.Contains("velkoz") || name.Contains("viktor") ||
                   name.Contains("vladimir") || name.Contains("fiddlesticks") || name.Contains("cassiopeia") ||
                   name.Contains("zoe") || name.Contains("lulu") || name.Contains("lux") ||
                   name.Contains("leblanc") || name.Contains("syndra") || name.Contains("orianna") ||
                   name.Contains("karma") || name.Contains("ryze") || name.Contains("twistedfate");
        }

        protected virtual bool IsTankChampion(string name)
        {
            return name.Contains("braum") || name.Contains("chogath") || name.Contains("warwick") ||
                   name.Contains("malphite") || name.Contains("sion") || name.Contains("volibear") ||
                   name.Contains("alistar") || name.Contains("amumu") || name.Contains("galio") ||
                   name.Contains("gnar") || name.Contains("maokai") || name.Contains("nautilus") ||
                   name.Contains("poppy") || name.Contains("rammus") || name.Contains("sejuani") ||
                   name.Contains("thresh") || name.Contains("taric");
        }

        protected virtual bool IsBruiserChampion(string name)
        {
            return name.Contains("darius") || name.Contains("xinzhao") || name.Contains("vi") ||
                   name.Contains("dr.mundo") || name.Contains("fiora") || name.Contains("irelia") ||
                   name.Contains("jax") || name.Contains("garen") || name.Contains("riven") ||
                   name.Contains("shyvana") || name.Contains("tryndamere") || name.Contains("udyr") ||
                   name.Contains("wukong") || name.Contains("yorick") || name.Contains("sett") ||
                   name.Contains("gwen") || name.Contains("sylas");
        }

        protected virtual bool IsAssassinChampion(string name)
        {
            return name.Contains("zed") || name.Contains("evelynn") || name.Contains("diana") ||
                   name.Contains("ekko") || name.Contains("talon") || name.Contains("leblanc") ||
                   name.Contains("katarina") || name.Contains("akali") || name.Contains("khazix") ||
                   name.Contains("shaco") || name.Contains("pyke") || name.Contains("qiyana");
        }

        protected virtual void AssignLaneWaypoints()
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

        protected Vector2 ApplyLaneOffset(Vector2 target)
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

        public virtual void OnUpdate(float diff)
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

            lastSpellCheckTime += diff / 1000f;
            stuckCheckTimer += diff / 1000f;

            UpdateBehavior(diff);
        }

        protected virtual void AutoLevelSkills()
        {
            if (champion.SkillPoints <= 0)
            {
                return;
            }

            if (role == Role.Unknown)
            {
                return;
            }

            int idx = champion.Stats.Level - 1;
            if (idx < 0 || idx >= skillPriority[role].Length)
            {
                return;
            }

            byte slot = skillPriority[role][idx];
            if (champion.Spells.ContainsKey(slot))
            {
                champion.LevelUpSpell(slot);
            }
        }

        protected virtual void TryBuyItems()
        {
            if (buildIndex >= buildOrder.Count)
            {
                return;
            }

            if (!IsAtBase())
            {
                return;
            }

            int targetItemId = buildOrder[buildIndex];

            if (champion.Inventory.GetAllItems().Any(item => item.ItemData.ItemId == targetItemId))
            {
                buildIndex++;
                return;
            }

            int beforeCount = champion.Inventory.GetAllItems().Count;

            champion.Shop.HandleItemBuyRequest(targetItemId);

            int afterCount = champion.Inventory.GetAllItems().Count;
            if (afterCount > beforeCount)
            {
                buildIndex++;
            }
        }

        protected virtual bool IsAtBase()
        {
            Vector2 basePos = champion.Team == TeamId.TEAM_BLUE
                ? new Vector2(0, 1000)
                : new Vector2(13300, 14600);
            return Vector2.Distance(champion.Position, basePos) < BASE_RADIUS;
        }

        protected virtual void UpdateBehavior(float diff)
        {
            CheckCombatState(diff);

            if (EmergencyBehavior() == BehaviorResult.Success)
            {
                return;
            }

            if (CombatBehavior() == BehaviorResult.Success)
            {
                return;
            }

            if (LaningBehavior() == BehaviorResult.Success)
            {
                return;
            }

            if (ResourceBehavior() == BehaviorResult.Success)
            {
                return;
            }

            MacroBehavior();
        }

        protected virtual BehaviorResult EmergencyBehavior()
        {
            if (champion.IsDead)
            {
                currentState = AIState.IDLE;
                return BehaviorResult.Success;
            }

            if (IsInEnemyTurretRange(champion.Position))
            {
                if (IsInCombat())
                {
                    float timeSinceLastCombat = GameTime() - lastCombatTime;
                    if (timeSinceLastCombat < 5.0f)
                    {
                        float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
                        if (hpPercent > RETREAT_HP_PERCENT)
                        {
                            if (ScanForTargets())
                            {
                                KeepFocusingTarget();
                                TryCastSpells();
                            }
                            return BehaviorResult.Success;
                        }
                    }
                }
                RetreatFromTurret();
                return BehaviorResult.Success;
            }

            float hpPercent2 = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
            if (hpPercent2 < 0.15f)
            {
                EmergencyHeal();
                return BehaviorResult.Success;
            }

            return BehaviorResult.Failure;
        }

        protected virtual void EmergencyHeal()
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

            if (!IsAtBase())
            {
                RetreatToBase();
            }
        }

        protected virtual BehaviorResult CombatBehavior()
        {
            if (!IsInCombat())
            {
                return BehaviorResult.Failure;
            }

            currentState = AIState.COMBAT;

            if (ScanForTargets())
            {
                var target = champion.TargetUnit;
                float threat = EvaluateThreat(target);

                if (threat > 100)
                {
                    FleeBehavior();
                    return BehaviorResult.Success;
                }
                else if (threat > 50)
                {
                    ChaseBehavior();
                    return BehaviorResult.Success;
                }

                KeepFocusingTarget();
                TryCastSpells();
                return BehaviorResult.Success;
            }

            return BehaviorResult.Failure;
        }

        protected virtual float EvaluateThreat(AttackableUnit target)
        {
            float selfThreat = CalculateSelfThreat();
            float enemyThreat = CalculateEnemyThreat(new List<AttackableUnit> { target });

            float threat = enemyThreat - selfThreat;

            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
            if (hpPercent < 0.3f)
            {
                threat += 50;
            }

            var nearbyEnemies = GetUnitsInRange(champion.Position, COMBAT_RANGE, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team != champion.Team && u is Champion)
                .ToList();
            threat += nearbyEnemies.Count * 80;

            return threat;
        }

        protected virtual float CalculateSelfThreat()
        {
            float threat = 0;

            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
            threat += hpPercent * 100;

            int readySpells = champion.Spells.Count(s => s.Value.CurrentCooldown <= 0);
            threat += readySpells * 50;

            float itemValue = champion.Inventory.GetAllItems().Sum(i => i.ItemData.Price);
            threat += itemValue / 100;

            return threat;
        }

        protected virtual float CalculateEnemyThreat(List<AttackableUnit> enemies)
        {
            float threat = 0;

            threat += enemies.Count * 80;

            float avgHpPercent = enemies.Average(e => e.Stats.CurrentHealth / e.Stats.HealthPoints.Total);
            threat += avgHpPercent * 50;

            int enemyReadySpells = enemies.OfType<Champion>()
                .Sum(c => c.Spells.Count(s => s.Value.CurrentCooldown <= 0));
            threat += enemyReadySpells * 30;

            return threat;
        }

        protected virtual void ChaseBehavior()
        {
            currentState = AIState.CHASE;
            var target = champion.TargetUnit;
            if (target == null || target.IsDead)
            {
                return;
            }

            float distance = Vector2.Distance(champion.Position, target.Position);
            float attackRange = champion.Stats.Range.Total;

            if (distance > attackRange + 1500f)
            {
                champion.CancelAutoAttack(false, true);
                champion.SetTargetUnit(null, true);
                currentState = AIState.LANING;
                return;
            }

            KeepFocusingTarget();
            TryCastSpells();
        }

        protected virtual void FleeBehavior()
        {
            currentState = AIState.FLEEING;
            champion.CancelAutoAttack(false, true);
            champion.SetTargetUnit(null, true);

            Vector2 fleeDirection = GetFleeDirection();
            Vector2 safePos = champion.Position + fleeDirection * 500f;

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

            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
            if (hpPercent < RETREAT_HP_PERCENT)
            {
                retreating = true;
                champion.Recall();
            }
        }

        protected virtual Vector2 GetFleeDirection()
        {
            Vector2 basePos = champion.Team == TeamId.TEAM_BLUE
                ? new Vector2(0, 1000)
                : new Vector2(13300, 14600);

            var nearbyEnemies = GetUnitsInRange(champion.Position, COMBAT_RANGE, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team != champion.Team && u is Champion)
                .ToList();

            Vector2 awayFromEnemies = new Vector2(0, 0);
            foreach (var enemy in nearbyEnemies)
            {
                Vector2 away = champion.Position - enemy.Position;
                if (away.LengthSquared() > 1f)
                {
                    awayFromEnemies += Vector2.Normalize(away);
                }
            }

            if (awayFromEnemies.LengthSquared() > 1f)
            {
                awayFromEnemies = Vector2.Normalize(awayFromEnemies);
            }
            else
            {
                awayFromEnemies = Vector2.Normalize(basePos - champion.Position);
            }

            Vector2 toBase = Vector2.Normalize(basePos - champion.Position);
            return Vector2.Normalize(awayFromEnemies * 0.6f + toBase * 0.4f);
        }

        protected virtual BehaviorResult LaningBehavior()
        {
            if (ScanForTargets())
            {
                KeepFocusingTarget();
                TryCastSpells();
                return BehaviorResult.Success;
            }

            currentState = AIState.LANING;
            MoveAlongLane(0);
            return BehaviorResult.Success;
        }

        protected virtual BehaviorResult ResourceBehavior()
        {
            float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
            float manaPercent = 1f;
            if (champion.Stats.ManaPoints.Total > 0)
            {
                manaPercent = champion.Stats.CurrentMana / champion.Stats.ManaPoints.Total;
            }

            if (hpPercent < RETREAT_HP_PERCENT || manaPercent < RETREAT_MANA_PERCENT)
            {
                if (!IsAtBase())
                {
                    retreating = true;
                    champion.Recall();
                    currentState = AIState.RECALLED;
                    return BehaviorResult.Success;
                }
            }

            if (retreating && IsAtBase() && hpPercent > 0.9f && manaPercent > 0.9f)
            {
                retreating = false;
                currentWaypoint = 1;
                currentState = AIState.RETURNING;
            }

            if (retreating)
            {
                RetreatToBase();
                return BehaviorResult.Success;
            }

            if (!IsAtBase() && champion.Stats.Gold >= RECALL_GOLD_THRESHOLD
                && buildIndex < buildOrder.Count && hpPercent > 0.5f)
            {
                champion.Recall();
                currentState = AIState.RECALLED;
                return BehaviorResult.Success;
            }

            return BehaviorResult.Failure;
        }

        protected virtual void MacroBehavior()
        {
            if (IsAtBase())
            {
                currentState = AIState.SHOPPING;
                return;
            }

            float gameTime = GameTime();
            if (gameTime < 600f)
            {
                MoveAlongLane(0);
                return;
            }

            CheckSupportRequest();
        }

        protected virtual void CheckSupportRequest()
        {
            var nearbyAllies = GetUnitsInRange(champion.Position, 2500f, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team == champion.Team && u is Champion)
                .ToList();

            foreach (var ally in nearbyAllies)
            {
                float allyHpPercent = ally.Stats.CurrentHealth / ally.Stats.HealthPoints.Total;
                if (allyHpPercent < 0.5f)
                {
                    var nearbyEnemies = GetUnitsInRange(ally.Position, 1000f, true)
                        .OfType<AttackableUnit>()
                        .Where(u => !u.IsDead && u.Team != champion.Team && u is Champion)
                        .ToList();

                    if (nearbyEnemies.Count > 0)
                    {
                        float supportScore = (1 - allyHpPercent) * 100 + nearbyEnemies.Count * 50;
                        if (supportScore > 50)
                        {
                            SupportAlly(ally);
                            return;
                        }
                    }
                }
            }
        }

        protected virtual void SupportAlly(AttackableUnit ally)
        {
            currentState = AIState.SUPPORTING;
            var path = GetPath(champion.Position, ally.Position);
            if (path != null && path.Count > 1)
            {
                champion.SetWaypoints(path);
            }
            else
            {
                champion.SetWaypoints(new List<Vector2> { champion.Position, ally.Position });
            }
            champion.UpdateMoveOrder(OrderType.MoveTo, true);
        }

        protected virtual bool IsInCombat()
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

        protected virtual void CheckCombatState(float diff)
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

        protected virtual bool IsInEnemyTurretRange(Vector2 position)
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

        protected virtual bool IsTargetInEnemyTurretRange(Vector2 target)
        {
            return IsInEnemyTurretRange(target);
        }

        protected virtual void RetreatFromTurret()
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

        protected virtual bool ScanForTargets()
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

            AttackableUnit bestTarget = SelectTarget();
            if (bestTarget != null)
            {
                champion.SetTargetUnit(bestTarget, true);
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

        protected virtual AttackableUnit SelectTarget()
        {
            float bestScore = float.MinValue;
            AttackableUnit bestTarget = null;
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

                if (u is BaseTurret && IsInEnemyTurretRange(u.Position))
                {
                    continue;
                }

                float score = EvaluateTarget(u);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = u;
                }
            }

            return bestTarget;
        }

        protected virtual float EvaluateTarget(AttackableUnit target)
        {
            float score = 0;

            if (target is Champion enemyChamp && enemyChamp.TargetUnit == champion)
            {
                score += 500;
            }

            float hpPercent = target.Stats.CurrentHealth / target.Stats.HealthPoints.Total;
            score += (1 - hpPercent) * 300;

            float distance = Vector2.Distance(champion.Position, target.Position);
            score += Math.Max(0, 200 - distance / 10);

            if (IsHighPriorityTarget(target))
            {
                score += 200;
            }

            if (target is Champion && ((Champion)target).IsMelee)
            {
                score += 50;
            }

            return score;
        }

        protected virtual bool IsHighPriorityTarget(AttackableUnit target)
        {
            if (!(target is Champion))
            {
                return false;
            }

            var champ = (Champion)target;
            string name = champ.Name.ToLower();
            return IsAdcChampion(name) || IsApChampion(name);
        }

        protected virtual void KeepFocusingTarget()
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

        protected virtual void TryCastSpells()
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

            var spells = champion.Spells.Values
                .Where(s => s != null && s.CastInfo.SpellLevel > 0 && s.CurrentCooldown == 0)
                .OrderByDescending(s => GetSpellPriority(s))
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
                    if (champion.Stats.CurrentMana / champion.Stats.ManaPoints.Total < 0.1f)
                    {
                        continue;
                    }
                }

                if (!ShouldCastSpell(spell, target))
                {
                    continue;
                }

                CastSpell(spell, target);
                return;
            }
        }

        protected virtual int GetSpellPriority(Spell spell)
        {
            if (spell.CastInfo.SpellSlot == 3)
            {
                return 100;
            }

            var targetingType = spell.SpellData.TargetingType;
            if (targetingType == TargetingType.Area || targetingType == TargetingType.Cone)
            {
                return 80;
            }

            return 50;
        }

        protected virtual bool ShouldCastSpell(Spell spell, AttackableUnit target)
        {
            if (random.NextDouble() > skillAccuracy)
            {
                return false;
            }

            var targetingType = spell.SpellData.TargetingType;
            if (targetingType == TargetingType.Self || targetingType == TargetingType.SelfAOE)
            {
                float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
                return hpPercent < 0.6f;
            }

            return true;
        }

        protected virtual void CastSpell(Spell spell, AttackableUnit target)
        {
            var castInfo = spell.CastInfo;
            castInfo.Targets.Clear();
            castInfo.AddTarget(target);

            spell.Cast(champion.Position, target.Position, target);
        }

        protected virtual void MoveAlongLane(float diff)
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

        protected virtual void CheckStuck(float diff)
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

        protected virtual void RetreatToBase()
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

        protected enum BehaviorResult
        {
            Success,
            Failure
        }
    }
}