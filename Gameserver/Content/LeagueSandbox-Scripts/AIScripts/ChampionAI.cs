using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using System.Linq;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.SpellNS;
using LeagueSandbox.GameServer.Inventory;

namespace AIScripts
{
    public class ChampionAI : IAIScript
    {
        public AIScriptMetaData AIScriptMetaData { get; set; } = new AIScriptMetaData();
        Champion champion;
        float aiUpdateTimer = 0f;
        float skillUseTimer = 0f;
        float buyItemTimer = 0f;
        const float AI_UPDATE_INTERVAL = 500f;
        const float SKILL_USE_INTERVAL = 2000f;
        const float BUY_ITEM_INTERVAL = 5000f;

        int[] startingItems = { 1036, 2003, 2003 };
        int[] damageItems = { 3074, 3077, 3053, 3031, 3153, 3035 };
        int[] tankItems = { 3065, 3022, 3068, 3043, 3006, 3092 };
        int[] apItems = { 3003, 3027, 3116, 3089, 3100, 3124 };
        int[] supportItems = { 3301, 3069, 3025, 3045, 3107, 3190 };

        bool hasBoughtStartingItems = false;
        bool hasReachedLane = false;
        Vector2 laneDestination;
        string laneRole;

        public void OnActivate(ObjAIBase owner)
        {
            champion = owner as Champion;
            DetermineLaneRole();
            SetLaneDestination();
        }

        public void OnUpdate(float diff)
        {
            if (champion == null || champion.IsDead)
            {
                return;
            }

            aiUpdateTimer += diff;
            skillUseTimer += diff;
            buyItemTimer += diff;

            if (aiUpdateTimer >= AI_UPDATE_INTERVAL)
            {
                aiUpdateTimer = 0;
                UpdateAI();
            }

            if (skillUseTimer >= SKILL_USE_INTERVAL)
            {
                skillUseTimer = 0;
                TryUseSkills();
            }

            if (buyItemTimer >= BUY_ITEM_INTERVAL)
            {
                buyItemTimer = 0;
                TryBuyItems();
            }
        }

        private void DetermineLaneRole()
        {
            string playerName = champion.Name;
            
            if (playerName.Contains("Top"))
            {
                laneRole = "Top";
            }
            else if (playerName.Contains("Mid"))
            {
                laneRole = "Mid";
            }
            else if (playerName.Contains("ADC"))
            {
                laneRole = "ADC";
            }
            else if (playerName.Contains("Support"))
            {
                laneRole = "Support";
            }
            else if (playerName.Contains("Jungle"))
            {
                laneRole = "Jungle";
            }
            else
            {
                laneRole = "Mid";
            }
        }

        private void SetLaneDestination()
        {
            bool isBlueTeam = champion.Team == TeamId.TEAM_BLUE;

            switch (laneRole)
            {
                case "Top":
                    laneDestination = isBlueTeam ? new Vector2(3800f, 10500f) : new Vector2(10000f, 3000f);
                    break;
                case "Mid":
                    laneDestination = isBlueTeam ? new Vector2(6000f, 8000f) : new Vector2(7800f, 5800f);
                    break;
                case "ADC":
                case "Support":
                    laneDestination = isBlueTeam ? new Vector2(8500f, 4500f) : new Vector2(5300f, 9300f);
                    break;
                case "Jungle":
                    laneDestination = isBlueTeam ? new Vector2(5500f, 6500f) : new Vector2(8300f, 7300f);
                    break;
                default:
                    laneDestination = isBlueTeam ? new Vector2(6000f, 8000f) : new Vector2(7800f, 5800f);
                    break;
            }
        }

        private void UpdateAI()
        {
            if (!hasReachedLane)
            {
                MoveToLane();
                return;
            }

            if (champion.TargetUnit == null || champion.TargetUnit.IsDead)
            {
                FindTarget();
            }
            else
            {
                float range = champion.Stats.Range.Total * champion.Stats.Range.Total * 2;
                if (Vector2.DistanceSquared(champion.Position, champion.TargetUnit.Position) > range)
                {
                    champion.SetTargetUnit(null, true);
                    FindTarget();
                }
            }
        }

        private void MoveToLane()
        {
            float distance = Vector2.Distance(champion.Position, laneDestination);
            
            if (distance < 500f)
            {
                hasReachedLane = true;
                return;
            }

            champion.SetTargetUnit(null, true);
            champion.SetWaypoints(new System.Collections.Generic.List<Vector2> { laneDestination });
            champion.UpdateMoveOrder(OrderType.MoveTo, true);
        }

        private void FindTarget()
        {
            float acquisitionRange = champion.Stats.AcquisitionRange.Total;
            var nearestObjects = GetUnitsInRange(champion.Position, acquisitionRange, true);

            AttackableUnit bestTarget = null;
            float bestDistance = float.MaxValue;

            foreach (var obj in nearestObjects)
            {
                if (obj is AttackableUnit unit &&
                    !unit.IsDead &&
                    unit.Team != champion.Team &&
                    unit.IsVisibleByTeam(champion.Team) &&
                    unit.Status.HasFlag(StatusFlags.Targetable))
                {
                    float distance = Vector2.DistanceSquared(champion.Position, unit.Position);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestTarget = unit;
                    }
                }
            }

            if (bestTarget != null)
            {
                champion.SetTargetUnit(bestTarget, true);
                champion.UpdateMoveOrder(OrderType.AttackTo, true);
            }
        }

        private void TryUseSkills()
        {
            if (champion.TargetUnit == null || champion.TargetUnit.IsDead)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                Spell spell;
                if (champion.Spells.TryGetValue((short)i, out spell))
                {
                    if (spell.CurrentCooldown <= 0 &&
                        champion.Stats.CurrentMana >= spell.CastInfo.ManaCost &&
                        CanCastSpellOnTarget(spell, champion.TargetUnit))
                    {
                        SpellCast(champion, i, SpellSlotType.SpellSlots, false, champion.TargetUnit, Vector2.Zero);
                        break;
                    }
                }
            }
        }

        private bool CanCastSpellOnTarget(Spell spell, AttackableUnit target)
        {
            float castRange = spell.SpellData.CastRange[0];
            if (castRange == 0)
            {
                castRange = champion.Stats.Range.Total;
            }

            return Vector2.DistanceSquared(champion.Position, target.Position) <= castRange * castRange;
        }

        private void TryBuyItems()
        {
            if (!hasBoughtStartingItems)
            {
                foreach (int itemId in startingItems)
                {
                    if (champion.Stats.Gold >= GetItemPrice(itemId))
                    {
                        champion.Shop.HandleItemBuyRequest(itemId);
                    }
                }
                hasBoughtStartingItems = true;
                return;
            }

            int[] itemList = GetItemListForChampion();
            
            foreach (int itemId in itemList)
            {
                float price = GetItemPrice(itemId);
                if (champion.Stats.Gold >= price)
                {
                    champion.Shop.HandleItemBuyRequest(itemId);
                    return;
                }
            }
        }

        private float GetItemPrice(int itemId)
        {
            var itemData = GetItemData(itemId);
            return itemData?.TotalPrice ?? float.MaxValue;
        }

        private int[] GetItemListForChampion()
        {
            string championName = champion.Name;
            
            string[] adChampions = { "MasterYi", "LeeSin", "Darius", "JarvanIV", "Zed" };
            string[] apChampions = { "Ahri" };
            string[] adcChampions = { "Ezreal", "Caitlyn" };
            string[] supportChampions = { "Sona", "Thresh" };

            if (adChampions.Contains(championName))
            {
                return damageItems;
            }
            if (apChampions.Contains(championName))
            {
                return apItems;
            }
            if (adcChampions.Contains(championName))
            {
                return damageItems;
            }
            if (supportChampions.Contains(championName))
            {
                return supportItems;
            }

            return damageItems;
        }
    }
}