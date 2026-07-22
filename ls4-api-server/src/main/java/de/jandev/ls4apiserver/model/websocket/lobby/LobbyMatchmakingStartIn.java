package de.jandev.ls4apiserver.model.websocket.lobby;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * Input model for LOBBY_MATCHMAKING_START message.
 * Contains the selected rune page and mastery page data.
 */
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyMatchmakingStartIn {

    /** Rune page data (can be null if not selected) */
    private Object runePage;

    /** Mastery page data (can be null if not selected) */
    private Object masteryPage;
}