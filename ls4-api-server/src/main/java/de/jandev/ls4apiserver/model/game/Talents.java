package de.jandev.ls4apiserver.model.game;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@JsonIgnoreProperties(ignoreUnknown = true)
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Talents {

    // === OFFENSE TREE (42xx) ===
    // Tier 1
    @JsonProperty("4211")
    private int doubleEdgedSword = 0;
    @JsonProperty("4212")
    private int fury = 0;
    @JsonProperty("4213")
    private int sorcery = 0;
    @JsonProperty("4214")
    private int butcher = 0;

    // Tier 2
    @JsonProperty("4221")
    private int feast = 0;
    @JsonProperty("4222")
    private int bruteForce = 0;
    @JsonProperty("4224")
    private int mentalForce = 0;

    // Tier 3
    @JsonProperty("4231")
    private int martialMastery = 0;
    @JsonProperty("4232")
    private int spellWeaving = 0;
    @JsonProperty("4233")
    private int arcaneMastery = 0;
    @JsonProperty("4234")
    private int bladeWeaving = 0;

    // Tier 4
    @JsonProperty("4241")
    private int reaper = 0;
    @JsonProperty("4242")
    private int dangerousGame = 0;
    @JsonProperty("4243")
    private int devastation = 0;
    @JsonProperty("4244")
    private int havoc = 0;

    // Tier 5
    @JsonProperty("4251")
    private int frenzy = 0;
    @JsonProperty("4252")
    private int spellWeavingUpgraded = 0;
    @JsonProperty("4253")
    private int bladeWeavingUpgraded = 0;

    // Tier 6 (Keystone)
    @JsonProperty("4262")
    private int warlordsMandate = 0;

    // === DEFENSE TREE (41xx) ===
    // Tier 1
    @JsonProperty("4111")
    private int block = 0;
    @JsonProperty("4112")
    private int recovery = 0;
    @JsonProperty("4113")
    private int unyielding = 0;
    @JsonProperty("4114")
    private int veteransScars = 0;

    // Tier 2
    @JsonProperty("4121")
    private int oppressor = 0;
    @JsonProperty("4122")
    private int toughSkin = 0;
    @JsonProperty("4123")
    private int bladedArmor = 0;
    @JsonProperty("4124")
    private int swiftness = 0;

    // Tier 3
    @JsonProperty("4131")
    private int perseverance = 0;
    @JsonProperty("4132")
    private int tenacity = 0;
    @JsonProperty("4133")
    private int runicShield = 0;
    @JsonProperty("4134")
    private int wandererDefense = 0;

    // Tier 4
    @JsonProperty("4141")
    private int legendaryGuardian = 0;
    @JsonProperty("4142")
    private int defender = 0;
    @JsonProperty("4143")
    private int secondWind = 0;
    @JsonProperty("4144")
    private int reinforcedArmor = 0;

    // Tier 5
    @JsonProperty("4151")
    private int reinOfSwiftness = 0;
    @JsonProperty("4152")
    private int runicShieldUpgraded = 0;
    @JsonProperty("4154")
    private int legendaryGuardianUpgraded = 0;

    // Tier 6 (Keystone)
    @JsonProperty("4162")
    private int bondOfStone = 0;

    // === UTILITY TREE (43xx) ===
    // Tier 1
    @JsonProperty("4311")
    private int phaseWalker = 0;
    @JsonProperty("4312")
    private int fleetOfFoot = 0;
    @JsonProperty("4313")
    private int meditation = 0;
    @JsonProperty("4314")
    private int summonersInsight = 0;

    // Tier 2
    @JsonProperty("4321")
    private int alchemist = 0;
    @JsonProperty("4322")
    private int culinaryMaster = 0;
    @JsonProperty("4323")
    private int runicAffinity = 0;
    @JsonProperty("4324")
    private int vampirism = 0;

    // Tier 3
    @JsonProperty("4331")
    private int greed = 0;
    @JsonProperty("4332")
    private int scavenge = 0;
    @JsonProperty("4333")
    private int meditationUpgraded = 0;
    @JsonProperty("4334")
    private int wealth = 0;

    // Tier 4
    @JsonProperty("4341")
    private int explorer = 0;
    @JsonProperty("4342")
    private int runicAffinityUpgraded = 0;
    @JsonProperty("4343")
    private int intelligence = 0;
    @JsonProperty("4344")
    private int windWalker = 0;

    // Tier 5
    @JsonProperty("4352")
    private int intelligenceUpgraded = 0;
    @JsonProperty("4353")
    private int wanderer = 0;

    // Tier 6 (Keystone)
    @JsonProperty("4362")
    private int stormraidersSurge = 0;
}
