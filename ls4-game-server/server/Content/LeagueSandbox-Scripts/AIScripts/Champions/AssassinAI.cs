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
    public class AssassinAI : BaseAI
    {
        private const float ASSASSIN_DETECT_RANGE = 1200.0f;
        private const float ASSASSIN_COMBAT_RANGE = 2000.0f;
        private const float ASSASSIN_KILL_THRESHOLD = 0.35f;
        private const float ASSASSIN_DASH_RANGE = 200.0f;
        private float lastDashTime = 0f;
        private float lastAssassinateTime = 0f;

        public override void OnActivate(ObjAIBase owner)
        {
            base.OnActivate(owner);
            role = Role.Jungle;
            skillAccuracy = 0.85f;
            reactionTime = 0.4f;
        }

        protected override void DetectRole()
        {
            role = Role.Jungle;
        }

        protected override List<int> GetBuildOrder(Role r)
        {
            return new List<int> { 1001, 3006, 1036, 3047, 3121, 3072, 3035, 3153 };
        }

        protected override AttackableUnit SelectTarget()
        {
            float bestScore = float.MinValue;
            AttackableUnit bestTarget = null;
            var nearbyUnits = GetUnitsInRange(champion.Position, ASSASSIN_DETECT_RANGE, true);

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

                float score = EvaluateAssassinTarget(u);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = u;
                }
            }

            return bestTarget;
        }

        private float EvaluateAssassinTarget(AttackableUnit target)
        {
            float score = 0;

            if (!(target is Champion))
            {
                return score;
            }

            var enemyChamp = (Champion)target;

            if (enemyChamp.TargetUnit == champion)
            {
                score += 200;
            }

            float hpPercent = target.Stats.CurrentHealth / target.Stats.HealthPoints.Total;
            score += (1 - hpPercent) * 400;

            if (hpPercent < ASSASSIN_KILL_THRESHOLD)
            {
                score += 300;
            }

            float distance = Vector2.Distance(champion.Position, target.Position);
            score += Math.Max(0, 150 - distance / 10);

            if (IsHighPriorityTarget(target))
            {
                score += 300;
            }

            if (!enemyChamp.IsMelee)
            {
                score += 150;
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

            if (distance > attackRange + ASSASSIN_DASH_RANGE)
            {
                TryUseDashSpell(target);
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

        private void TryUseDashSpell(AttackableUnit target)
        {
            float timeSinceLastDash = GameTime() - lastDashTime;
            if (timeSinceLastDash < 5.0f)
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

                if (range > 0 && distance <= range + ASSASSIN_DASH_RANGE)
                {
                    var targetingType = spell.SpellData.TargetingType;
                    if (targetingType == TargetingType.Target || targetingType == TargetingType.TargetOrLocation)
                    {
                        lastDashTime = GameTime();
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
            if (lastSpellCheckTime < SPELL_CHECK_INTERVAL * 0.7f)
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
            float targetHpPercent = target.Stats.CurrentHealth / target.Stats.HealthPoints.Total;

            var spells = champion.Spells.Values
                .Where(s => s != null && s.CastInfo.SpellLevel > 0 && s.CurrentCooldown == 0)
                .OrderByDescending(s => GetAssassinSpellPriority(s))
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

                if (spell.CastInfo.SpellSlot == 3 && targetHpPercent > ASSASSIN_KILL_THRESHOLD)
                {
                    continue;
                }

                CastSpell(spell, target);

                if (spell.CastInfo.SpellSlot == 3)
                {
                    lastAssassinateTime = GameTime();
                }

                return;
            }
        }

        private int GetAssassinSpellPriority(Spell spell)
        {
            if (spell.CastInfo.SpellSlot == 3)
            {
                return 100;
            }

            var targetingType = spell.SpellData.TargetingType;
            if (targetingType == TargetingType.Target || targetingType == TargetingType.TargetOrLocation)
            {
                return 90;
            }

            if (targetingType == TargetingType.Area || targetingType == TargetingType.Cone)
            {
                return 80;
            }

            return 50;
        }

        protected override void ChaseBehavior()
        {
            var target = champion.TargetUnit;
            if (target == null || target.IsDead)
            {
                return;
            }

            float distance = Vector2.Distance(champion.Position, target.Position);
            float attackRange = champion.Stats.Range.Total;

            if (distance > attackRange + ASSASSIN_DASH_RANGE)
            {
                TryUseDashSpell(target);
            }

            float targetHpPercent = target.Stats.CurrentHealth / target.Stats.HealthPoints.Total;
            if (targetHpPercent > ASSASSIN_KILL_THRESHOLD && distance > attackRange + 500f)
            {
                champion.CancelAutoAttack(false, true);
                champion.SetTargetUnit(null, true);
                currentState = AIState.LANING;
                return;
            }

            base.ChaseBehavior();
        }

        protected override void FleeBehavior()
        {
            float timeSinceLastAssassinate = GameTime() - lastAssassinateTime;
            if (timeSinceLastAssassinate < 3.0f)
            {
                return;
            }

            base.FleeBehavior();
        }

        protected override void CheckCombatState(float diff)
        {
            var nearbyUnits = GetUnitsInRange(champion.Position, ASSASSIN_COMBAT_RANGE, true);
            foreach (var unit in nearbyUnits)
            {
                if (!(unit is AttackableUnit u)
                    || u.IsDead
                    || u.Team == champion.Team
                    || !u.IsVisibleByTeam(champion.Team))
                {
                    continue;
                }

                if (u is Champion || u is BaseTurret)
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
            if (hpPercent < ASSASSIN_KILL_THRESHOLD)
            {
                threat += 100;
            }

            return threat;
        }
    }
}