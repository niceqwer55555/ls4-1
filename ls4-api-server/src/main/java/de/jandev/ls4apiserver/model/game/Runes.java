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
public class Runes {

    public static final int MARK_AD = 5245;
    public static final int MARK_AP = 5275;
    public static final int MARK_ARMOR = 5317;
    public static final int MARK_MAGIC_RESIST = 5250;
    public static final int MARK_ATTACK_SPEED = 5231;
    public static final int MARK_CRIT_CHANCE = 5272;
    public static final int MARK_ARMOR_PEN = 5224;

    public static final int SEAL_ARMOR = 5317;
    public static final int SEAL_HEALTH = 5340;
    public static final int SEAL_MANA_REGEN = 5300;
    public static final int SEAL_MAGIC_RESIST = 5302;
    public static final int SEAL_HP_REGEN = 5327;
    public static final int SEAL_MOVEMENT_SPEED = 5341;

    public static final int GLYPH_MAGIC_RESIST = 5289;
    public static final int GLYPH_COOLDOWN = 5273;
    public static final int GLYPH_AP = 5265;
    public static final int GLYPH_MANA_REGEN = 5290;
    public static final int GLYPH_MANA = 5270;
    public static final int GLYPH_ATTACK_SPEED = 5299;

    public static final int QUINTESSENCE_AD = 5335;
    public static final int QUINTESSENCE_AP = 5337;
    public static final int QUINTESSENCE_MOVEMENT_SPEED = 5310;
    public static final int QUINTESSENCE_LIFESTEAL = 5313;
    public static final int QUINTESSENCE_ARMOR = 5357;
    public static final int QUINTESSENCE_MAGIC_RESIST = 5359;
    public static final int QUINTESSENCE_ATTACK_SPEED = 5338;
    public static final int QUINTESSENCE_HP = 5342;
    public static final int QUINTESSENCE_HP_REGEN = 5343;

    @JsonProperty("1")
    private int one = MARK_AD;
    @JsonProperty("2")
    private int two = MARK_AD;
    @JsonProperty("3")
    private int three = MARK_AD;
    @JsonProperty("4")
    private int four = MARK_AD;
    @JsonProperty("5")
    private int five = MARK_AD;
    @JsonProperty("6")
    private int six = MARK_AD;
    @JsonProperty("7")
    private int seven = MARK_AD;
    @JsonProperty("8")
    private int eight = MARK_AD;
    @JsonProperty("9")
    private int nine = MARK_AD;

    @JsonProperty("10")
    private int ten = SEAL_ARMOR;
    @JsonProperty("11")
    private int eleven = SEAL_ARMOR;
    @JsonProperty("12")
    private int twelve = SEAL_ARMOR;
    @JsonProperty("13")
    private int thirteen = SEAL_ARMOR;
    @JsonProperty("14")
    private int fourteen = SEAL_ARMOR;
    @JsonProperty("15")
    private int fifteen = SEAL_ARMOR;
    @JsonProperty("16")
    private int sixteen = SEAL_ARMOR;
    @JsonProperty("17")
    private int seventeen = SEAL_ARMOR;
    @JsonProperty("18")
    private int eighteen = SEAL_ARMOR;

    @JsonProperty("19")
    private int nineteen = GLYPH_MAGIC_RESIST;
    @JsonProperty("20")
    private int twenty = GLYPH_MAGIC_RESIST;
    @JsonProperty("21")
    private int twentyOne = GLYPH_MAGIC_RESIST;
    @JsonProperty("22")
    private int twentyTwo = GLYPH_MAGIC_RESIST;
    @JsonProperty("23")
    private int twentyThree = GLYPH_MAGIC_RESIST;
    @JsonProperty("24")
    private int twentyFour = GLYPH_MAGIC_RESIST;
    @JsonProperty("25")
    private int twentyFive = GLYPH_MAGIC_RESIST;
    @JsonProperty("26")
    private int twentySix = GLYPH_MAGIC_RESIST;
    @JsonProperty("27")
    private int twentySeven = GLYPH_MAGIC_RESIST;

    @JsonProperty("28")
    private int twentyEight = QUINTESSENCE_AD;
    @JsonProperty("29")
    private int twentyNine = QUINTESSENCE_AD;
    @JsonProperty("30")
    private int thirty = QUINTESSENCE_AD;

}
