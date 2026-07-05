package de.jandev.ls4apiserver.model.lobby;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class LobbyTypeTest {

    @Test
    void shouldExposeOdinLobbyTypesForLauncherAndGameServer() {
        assertEquals(8, LobbyType.ODIN_BLIND.getMapType());
        assertEquals(8, LobbyType.ODIN_DRAFT.getMapType());
        assertEquals(8, LobbyType.ODIN_BOT_BLIND.getMapType());
        assertEquals(8, LobbyType.ODIN_BOT_DRAFT.getMapType());
    }
}
