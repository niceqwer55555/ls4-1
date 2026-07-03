package de.jandev.ls4apiserver.model.lobby;

import lombok.Getter;

@Getter
public enum LobbyType {
    SUMMONERS_RIFT_DRAFT(5, 3, 1), SUMMONERS_RIFT_BLIND(5, 0, 1),
    TWISTED_TREELINE_DRAFT(3, 3, 10), TWISTED_TREELINE_BLIND(3, 0, 10),
    ARAM_BLIND(5, 0, 12);
    // CUSTOM SIZE IS ALWAYS 10 currently (Update: I have no clue what this means.)

    private final int teamSize;
    private final int maxBans;
    private final int mapType;

    // [0] = typeof(FlatTestMap),
    // [1] = typeof(SummonersRift),
    // [2] = typeof(HarrowingRift),
    // [3] = typeof(ProvingGrounds),
    // [4] = typeof(TwistedTreeline),
    // [6] = typeof(WinterRift),
    // [8] = typeof(CrystalScar),
    // [10] = typeof(NewTwistedTreeline),
    // [11] = typeof(NewSummonersRift),
    // [12] = typeof(HowlingAbyss)
    // [14] = typeof(ButchersBridge)

    LobbyType(int teamSize, int maxBans, int mapType) {
        this.teamSize = teamSize;
        this.maxBans = maxBans;
        this.mapType = mapType;
    }
}
