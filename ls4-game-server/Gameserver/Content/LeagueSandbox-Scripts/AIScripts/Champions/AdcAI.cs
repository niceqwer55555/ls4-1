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
    public class AdcAI : BaseAI
    {
        private const float ADC_DETECT_RANGE = 700.0f;
        private const float ADC_COMBAT_RANGE = 1400.0f;
        private const float ADC_KEEP_DISTANCE = 500.0f;
        private float lastEscapeTime = 0f;
        private float lastAutoAttackTime = 0f;

        public override void OnActivate(ObjAIBase owner)
        {
            base.OnActivate(owner);
            role = Role.ADC;
            skillAccuracy = 0.8f;
            csAccuracy = 0.9f;
        }

        protected override void DetectRole()
        {
            role = Role.ADC;
        }

        protected override List<int> GetBuildOrder(Role r)
        {
            return new List<int> { 1001, 3006, 1036, 3031, 3046, 3072, 3035, 3074 };
        }

        protected override AttackableUnit SelectTarget()
        {
            float bestScore = float.MinValue;
            AttackableUnit bestTarget = null;
            var nearbyUnits = GetUnitsInRange(champion.Position, ADC_DETECT_RANGE, true);

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

                float score = EvaluateAdcTarget(u);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = u;
                }
            }

            return bestTarget;
        }

        private float EvaluateAdcTarget(AttackableUnit target)
        {
            float score = 0;

            if (target is Champion enemyChamp && enemyChamp.TargetUnit == champion)
            {
                score += 400;
            }

            float hpPercent = target.Stats.CurrentHealth / target.Stats.HealthPoints.Total;
            score += (1 - hpPercent) * 300;

            float distance = Vector2.Distance(champion.Position, target.Position);
            float attackRange = champion.Stats.Range.Total;
            score += Math.Max(0, 150 - Math.Abs(distance - attackRange) / 5);

            if (IsHighPriorityTarget(target))
            {
                score += 250;
            }

            if (target is Champion && ((Champion)target).IsMelee)
            {
                score += 80;
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

            if (target is Champion && distance < ADC_KEEP_DISTANCE)
            {
                TryEscape(target);
                return;
            }

            if (distance > attackRange + 50f)
            {
                Vector2 optimalPosition = target.Position - Vector2.Normalize(target.Position - champion.Position) * (attackRange - 50f);
                var path = GetPath(champion.Position, optimalPosition);
                if (path != null && path.Count > 1)
                {
                    champion.SetWaypoints(path);
                }
                else
                {
                    champion.SetWaypoints(new List<Vector2> { champion.Position, optimalPosition });
                }
                champion.UpdateMoveOrder(OrderType.MoveTo, true);
            }
            else
            {
                champion.UpdateMoveOrder(OrderType.AttackMove, true);
                ExecuteAutoAttack(target);
            }
        }

        private void ExecuteAutoAttack(AttackableUnit target)
        {
            float gameTime = GameTime();
            float attackSpeedMultiplier = champion.Stats.AttackSpeedMultiplier.Total;
            float baseAttackInterval = 1.0f;
            float attackInterval = baseAttackInterval / attackSpeedMultiplier;

            if (gameTime - lastAutoAttackTime >= attackInterval)
            {
                lastAutoAttackTime = gameTime;
                if (champion.AutoAttackSpell.State == SpellState.STATE_READY)
                {
                    champion.AutoAttackSpell.Cast(target.Position, target.Position, target);
                }
            }
        }

        protected override void TryCastSpells()
        {
            if (lastSpellCheckTime < SPELL_CHECK_INTERVAL * 1.2f)
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
            float attackRange = champion.Stats.Range.Total;

            if (target is Champion && distance < ADC_KEEP_DISTANCE)
            {
                TryEscape(target);
                return;
            }

            var spells = champion.Spells.Values
                .Where(s => s != null && s.CastInfo.SpellLevel > 0 && s.CurrentCooldown == 0)
                .OrderByDescending(s => GetAdcSpellPriority(s))
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
                    float hpPercent = champion.Stats.CurrentHealth / champion.Stats.HealthPoints.Total;
                    if (hpPercent >= 0.6f)
                    {
                        continue;
                    }
                }

                CastSpell(spell, target);
                return;
            }
        }

        private int GetAdcSpellPriority(Spell spell)
        {
            if (spell.CastInfo.SpellSlot == 3)
            {
                return 100;
            }

            var targetingType = spell.SpellData.TargetingType;
            if (targetingType == TargetingType.Area)
            {
                return 90;
            }

            if (targetingType == TargetingType.Area || targetingType == TargetingType.Cone)
            {
                return 80;
            }

            return 50;
        }

        private void TryEscape(AttackableUnit threat)
        {
            float timeSinceLastEscape = GameTime() - lastEscapeTime;
            if (timeSinceLastEscape < 8.0f)
            {
                return;
            }

            foreach (var spell in champion.Spells.Values)
            {
                if (spell == null || spell.CastInfo.SpellLevel <= 0 || spell.CurrentCooldown > 0)
                {
                    continue;
                }

                var targetingType = spell.SpellData.TargetingType;
                if (targetingType == TargetingType.Self || targetingType == TargetingType.SelfAOE)
                {
                    var castInfo = spell.CastInfo;
                    castInfo.Targets.Clear();
                    castInfo.AddTarget(champion);
                    spell.Cast(champion.Position, champion.Position, champion);
                    lastEscapeTime = GameTime();
                    return;
                }
            }

            Vector2 away = champion.Position - threat.Position;
            if (away.LengthSquared() < 1f)
            {
                away = champion.Team == TeamId.TEAM_BLUE
                        ? new Vector2(-1, -1)
                        : new Vector2(1, 1);
            }
            away = Vector2.Normalize(away);
            Vector2 safePos = champion.Position + away * ADC_KEEP_DISTANCE;

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
            lastEscapeTime = GameTime();
        }

        protected override void CheckCombatState(float diff)
        {
            var nearbyUnits = GetUnitsInRange(champion.Position, ADC_COMBAT_RANGE, true);
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

            float distance = Vector2.Distance(champion.Position, target.Position);
            if (distance < ADC_KEEP_DISTANCE && target is Champion)
            {
                threat += 80;
            }

            return threat;
        }
    }
}