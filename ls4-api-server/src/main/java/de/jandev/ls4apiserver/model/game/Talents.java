package de.jandev.ls4apiserver.model.game;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Talents {

    @JsonProperty("4211")
    private int brutalForce = 3;
    @JsonProperty("4212")
    private int fury = 0;
    @JsonProperty("4213")
    private int sorcery = 0;
    @JsonProperty("4214")
    private int savagery = 0;

    @JsonProperty("4221")
    private int feast = 0;
    @JsonProperty("4222")
    private int veteranScars = 3;
    @JsonProperty("4223")
    private int resilience = 0;
    @JsonProperty("4224")
    private int butcher = 0;

    @JsonProperty("4231")
    private int meditation = 0;
    @JsonProperty("4232")
    private int perseverance = 3;
    @JsonProperty("4233")
    private int expandedMind = 0;
    @JsonProperty("4234")
    private int vampirism = 0;

    @JsonProperty("4241")
    private int wrath = 3;
    @JsonProperty("4242")
    private int arcaneKnowledge = 0;
    @JsonProperty("4243")
    private int precision = 0;
    @JsonProperty("4244")
    private int intelligence = 0;

    @JsonProperty("4111")
    private int healingMastery = 0;
    @JsonProperty("4112")
    private int spellWeaving = 0;
    @JsonProperty("4113")
    private int bladedArmor = 0;
    @JsonProperty("4114")
    private int dangerZone = 0;
    @JsonProperty("4115")
    private int doubleEdgedSword = 0;
    @JsonProperty("4116")
    private int naturalTalent = 0;

    @JsonProperty("4121")
    private int warlord = 0;
    @JsonProperty("4122")
    private int berserker = 0;
    @JsonProperty("4123")
    private int spellPiercing = 0;
    @JsonProperty("4124")
    private int executioner = 3;
    @JsonProperty("4125")
    private int archmage = 0;
    @JsonProperty("4126")
    private int assassin = 0;

    @JsonProperty("4131")
    private int juggernaut = 3;
    @JsonProperty("4132")
    private int demolitionist = 0;
    @JsonProperty("4133")
    private int fearless = 0;
    @JsonProperty("4134")
    private int siegeMaster = 0;
    @JsonProperty("4135")
    private int tenacity = 0;
    @JsonProperty("4136")
    private int guardianAngel = 0;

    @JsonProperty("4141")
    private int thunderlordsDecree = 1;
    @JsonProperty("4142")
    private int stormraidersSurge = 0;
    @JsonProperty("4143")
    private int deathfireTouch = 0;
    @JsonProperty("4144")
    private int fervorOfBattle = 0;
    @JsonProperty("4145")
    private int warlordsBloodlust = 0;
    @JsonProperty("4146")
    private int graspOfTheUndying = 0;
}