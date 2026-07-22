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
public class Runes {

    // === MARKS (印记) - Tier 3 ===
    public static final int MARK_AD = 5245;            // 高级攻击力印记
    public static final int MARK_ARMOR_PEN = 5253;     // 高级护甲穿透印记
    public static final int MARK_MAGIC_PEN = 5273;     // 高级法术穿透印记
    public static final int MARK_ATTACK_SPEED = 5247;   // 高级攻击速度印记
    public static final int MARK_CRIT_CHANCE = 5251;    // 高级暴击几率印记
    public static final int MARK_MAGIC_RESIST = 5259;   // 高级魔法抗性印记
    public static final int MARK_COOLDOWN = 5265;       // 高级冷却缩减印记
    public static final int MARK_HYBRID_PEN = 5402;     // 高级混合穿透印记

    // === SEALS (符印) - Tier 3 ===
    public static final int SEAL_ARMOR = 5317;          // 高级护甲符印
    public static final int SEAL_HEALTH = 5340;         // 高级生命值符印
    public static final int SEAL_MANA_REGEN = 5300;     // 高级法力回复符印
    public static final int SEAL_MAGIC_RESIST = 5302;   // 高级魔法抗性符印
    public static final int SEAL_HP_REGEN = 5327;       // 高级生命回复符印

    // === GLYPHS (雕文) - Tier 3 ===
    public static final int GLYPH_MAGIC_RESIST = 5289;  // 高级魔法抗性雕文
    public static final int GLYPH_COOLDOWN = 5295;      // 高级冷却缩减雕文
    public static final int GLYPH_AP = 5297;            // 高级法术强度雕文
    public static final int GLYPH_MANA_REGEN = 5290;    // 高级法力回复雕文
    public static final int GLYPH_MANA = 5299;          // 高级法力值雕文
    public static final int GLYPH_ATTACK_SPEED = 5277;  // 高级攻击速度雕文

    // === QUINTESSENCES (精华) - Tier 3 ===
    public static final int QUINTESSENCE_AD = 5335;            // 高级攻击力精华
    public static final int QUINTESSENCE_AP = 5357;            // 高级法术强度精华
    public static final int QUINTESSENCE_MOVEMENT_SPEED = 5365; // 高级移动速度精华
    public static final int QUINTESSENCE_LIFESTEAL = 5412;     // 高级生命偷取精华
    public static final int QUINTESSENCE_ARMOR = 5347;         // 高级护甲精华
    public static final int QUINTESSENCE_MAGIC_RESIST = 5349;  // 高级魔法抗性精华
    public static final int QUINTESSENCE_ATTACK_SPEED = 5337;  // 高级攻击速度精华
    public static final int QUINTESSENCE_HP = 5345;            // 高级生命值精华
    public static final int QUINTESSENCE_HP_REGEN = 5351;      // 高级生命回复精华

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
