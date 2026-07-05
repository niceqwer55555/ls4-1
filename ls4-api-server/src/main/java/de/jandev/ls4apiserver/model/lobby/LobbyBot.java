package de.jandev.ls4apiserver.model.lobby;

import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.UUID;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyBot {

    private String botId = UUID.randomUUID().toString();
    private String name;
    private String championId;
    private String championDisplayName;
    private BotDifficulty difficulty;
    private LobbyTeam team;
    private String role;
}
