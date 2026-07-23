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
    public class ApMageAI : BaseAI
    {
        private const float AP_DETECT_RANGE = 1000.0f;
        private const float AP_COMBAT_RANGE = 1600.0f;
        private const float AP_WAVE_CLEAR_THRESHOLD = 3;
        private const float AP_SAFE_DISTANCE = 600.0f;
        private float lastWaveClearTime = 0f;
        private float lastSupportTime = 0f;

        public override void OnActivate(ObjAIBase owner)
        {
            base.OnActivate(owner);
            role = Role.Mid;
            skillAccuracy = 0.75f;
        }

        protected override void DetectRole()
        {
            role = Role.Mid;
        }

        protected override List<int> GetBuildOrder(Role r)
        {
            return new List<int> { 1001, 3020, 1058, 3089, 3135, 3157, 3041, 3116 };
        }

        protected override AttackableUnit SelectTarget()
        {
            float bestScore = float.MinValue;
            AttackableUnit bestTarget = null;
            var nearbyUnits = GetUnitsInRange(champion.Position, AP_DETECT_RANGE, true);

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

                float score = EvaluateApTarget(u);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = u;
                }
            }

            return bestTarget;
        }

        private float EvaluateApTarget(AttackableUnit target)
        {
            float score = 0;

            if (target is Champion enemyChamp && enemyChamp.TargetUnit == champion)
            {
                score += 300;
            }

            float hpPercent = target.Stats.CurrentHealth / target.Stats.HealthPoints.Total;
            score += (1 - hpPercent) * 250;

            float distance = Vector2.Distance(champion.Position, target.Position);
            score += Math.Max(0, 180 - distance / 8);

            if (IsHighPriorityTarget(target))
            {
                score += 200;
            }

            if (target is Champion)
            {
                var champ = (Champion)target;
                if (!champ.IsMelee)
                {
                    score += 100;
                }
            }

            return score;
        }

        protected override void TryCastSpells()
        {
            if (lastSpellCheckTime < SPELL_CHECK_INTERVAL * 0.8f)
            {
                return;
            }
            lastSpellCheckTime = 0f;

            float timeSinceLastWaveClear = GameTime() - lastWaveClearTime;
            if (timeSinceLastWaveClear >= 8.0f)
            {
                if (TryWaveClear())
                {
                    return;
                }
            }

            var target = champion.TargetUnit;
            if (target == null || target.IsDead)
            {
                return;
            }

            float distance = Vector2.Distance(champion.Position, target.Position);

            if (target is Champion && distance < AP_SAFE_DISTANCE)
            {
                TryRetreatFromMelee(target);
                return;
            }

            var spells = champion.Spells.Values
                .Where(s => s != null && s.CastInfo.SpellLevel > 0 && s.CurrentCooldown == 0)
                .OrderByDescending(s => GetApSpellPriority(s))
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
                    if (champion.Stats.CurrentMana / champion.Stats.ManaPoints.Total < 0.15f)
                    {
                        continue;
                    }
                }

                CastSpell(spell, target);
                return;
            }
        }

        private int GetApSpellPriority(Spell spell)
        {
            if (spell.CastInfo.SpellSlot == 3)
            {
                return 100;
            }

            var targetingType = spell.SpellData.TargetingType;
            if (targetingType == TargetingType.Area || targetingType == TargetingType.Cone)
            {
                return 90;
            }

            if (targetingType == TargetingType.Location)
            {
                return 80;
            }

            return 50;
        }

        private bool TryWaveClear()
        {
            var nearbyMinions = GetUnitsInRange(champion.Position, AP_DETECT_RANGE, true)
                .OfType<Minion>()
                .Where(m => !m.IsDead && m.Team != champion.Team)
                .ToList();

            if (nearbyMinions.Count < AP_WAVE_CLEAR_THRESHOLD)
            {
                return false;
            }

            Vector2 center = new Vector2(0, 0);
            foreach (var minion in nearbyMinions)
            {
                center += minion.Position;
            }
            center /= nearbyMinions.Count;

            foreach (var spell in champion.Spells.Values)
            {
                if (spell == null || spell.CastInfo.SpellLevel <= 0 || spell.CurrentCooldown > 0)
                {
                    continue;
                }

                float range = spell.GetCurrentCastRange();
                float distance = Vector2.Distance(champion.Position, center);

                if (range > 0 && distance <= range)
                {
                    var targetingType = spell.SpellData.TargetingType;
                    if (targetingType == TargetingType.Area || targetingType == TargetingType.Location || targetingType == TargetingType.Cone)
                    {
                        if (champion.Stats.ManaPoints.Total > 0)
                        {
                            float manaCost = spell.SpellData.ManaCost[spell.CastInfo.SpellLevel];
                            if (champion.Stats.CurrentMana < manaCost)
                            {
                                continue;
                            }
                        }

                        var castInfo = spell.CastInfo;
                        castInfo.Targets.Clear();
                        spell.Cast(champion.Position, center);
                        lastWaveClearTime = GameTime();
                        return true;
                    }
                }
            }

            return false;
        }

        private void TryRetreatFromMelee(AttackableUnit threat)
        {
            Vector2 away = champion.Position - threat.Position;
            if (away.LengthSquared() < 1f)
            {
                away = champion.Team == TeamId.TEAM_BLUE
                        ? new Vector2(-1, -1)
                        : new Vector2(1, 1);
            }
            away = Vector2.Normalize(away);
            Vector2 safePos = champion.Position + away * AP_SAFE_DISTANCE;

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
        }

        protected override void CheckCombatState(float diff)
        {
            var nearbyUnits = GetUnitsInRange(champion.Position, AP_COMBAT_RANGE, true);
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

        protected override void MacroBehavior()
        {
            base.MacroBehavior();

            float gameTime = GameTime();
            if (gameTime > 600f && gameTime - lastSupportTime > 30f)
            {
                CheckSupportRequest();
            }
        }

        protected override void CheckSupportRequest()
        {
            float gameTime = GameTime();
            if (gameTime < 600f)
            {
                return;
            }

            var nearbyAllies = GetUnitsInRange(champion.Position, 3000f, true)
                .OfType<AttackableUnit>()
                .Where(u => !u.IsDead && u.Team == champion.Team && u is Champion)
                .ToList();

            foreach (var ally in nearbyAllies)
            {
                float allyHpPercent = ally.Stats.CurrentHealth / ally.Stats.HealthPoints.Total;
                if (allyHpPercent < 0.4f)
                {
                    var nearbyEnemies = GetUnitsInRange(ally.Position, 1200f, true)
                        .OfType<AttackableUnit>()
                        .Where(u => !u.IsDead && u.Team != champion.Team && u is Champion)
                        .ToList();

                    if (nearbyEnemies.Count > 0)
                    {
                        float supportScore = (1 - allyHpPercent) * 80 + nearbyEnemies.Count * 40;
                        if (supportScore > 40)
                        {
                            SupportAlly(ally);
                            lastSupportTime = gameTime;
                            return;
                        }
                    }
                }
            }
        }
    }
}