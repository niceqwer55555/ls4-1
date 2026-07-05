package de.jandev.ls4apiserver.model.websocket.lobby;

import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.lobby.BotDifficulty;
import de.jandev.ls4apiserver.model.lobby.LobbyBot;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyBotOut {

    private String botId;
    private String name;
    private String championId;
    private String championDisplayName;
    private BotDifficulty difficulty;
    private LobbyTeam team;
    private String role;

    public LobbyBotOut(LobbyBot bot) {
        this.botId = bot.getBotId();
        this.name = bot.getName();
        this.championId = bot.getChampionId();
        this.championDisplayName = bot.getChampionDisplayName();
        this.difficulty = bot.getDifficulty();
        this.team = bot.getTeam();
        this.role = bot.getRole();
    }
}
