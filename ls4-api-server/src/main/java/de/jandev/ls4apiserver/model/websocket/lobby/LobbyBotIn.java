package de.jandev.ls4apiserver.model.websocket.lobby;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class LobbyBotIn {

    private String championId;
    private String difficulty;
    private String team;
    private String role;
}
