package de.jandev.ls4apiserver.model.lobby;

import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.game.Runes;
import de.jandev.ls4apiserver.model.game.Talents;
import de.jandev.ls4apiserver.model.user.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.stream.Collectors;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Lobby {

    private String uuid = UUID.randomUUID().toString();

    private LobbyType lobbyType;

    private List<User> members = new CopyOnWriteArrayList<>();

    private List<LobbyBot> bots = new CopyOnWriteArrayList<>();

    private Map<User, InviteStatus> invited = new ConcurrentHashMap<>();

    private User owner;

    private LocalDateTime joinedQueueTime;

    private boolean inQueue;

    private boolean isCustom;

    private boolean alteredDuringAccept;

    /** Temporary storage for rune page data per user, set when matchmaking starts */
    private Map<String, Runes> memberRunePages = new ConcurrentHashMap<>();

    /** Temporary storage for mastery page data per user, set when matchmaking starts */
    private Map<String, Talents> memberMasteryPages = new ConcurrentHashMap<>();

    @Override
    public String toString() {
        return "Lobby{" +
                "uuid='" + uuid + '\'' +
                ", lobbyType=" + lobbyType +
                ", members=" + members +
                ", bots=" + bots +
                ", invited=" + invited +
                ", owner=" + owner +
                ", joinedQueueTime=" + joinedQueueTime +
                ", inQueue=" + inQueue +
                ", isCustom=" + isCustom +
                ", alteredDuringAccept=" + alteredDuringAccept +
                '}';
    }

    public List<User> getTeam1() {
        return members.stream().filter(c -> c.getLobbyTeam() == LobbyTeam.TEAM1).collect(Collectors.toList());
    }

    public List<User> getTeam2() {
        return members.stream().filter(c -> c.getLobbyTeam() == LobbyTeam.TEAM2).collect(Collectors.toList());
    }
}
