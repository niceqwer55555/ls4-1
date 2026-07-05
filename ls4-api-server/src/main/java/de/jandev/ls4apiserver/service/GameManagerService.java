package de.jandev.ls4apiserver.service;

import com.fasterxml.jackson.core.util.DefaultPrettyPrinter;
import com.fasterxml.jackson.databind.ObjectMapper;
import de.jandev.ls4apiserver.model.champselect.GameLobby;
import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.champselect.SummonerSpell;
import de.jandev.ls4apiserver.model.game.*;
import de.jandev.ls4apiserver.model.websocket.champselect.GameStartOut;
import de.jandev.ls4apiserver.utility.LogMessage;
import de.jandev.ls4apiserver.utility.ServerKillTask;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.scheduling.TaskScheduler;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import javax.annotation.PostConstruct;
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.Instant;
import java.time.LocalDate;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ScheduledFuture;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@Service
public class GameManagerService {
    // This service is temporary and will be replaced by the automatic pterodactyl server spin-up service later on
    // Issues are among others: Static blowfish keys, static ranks, static ribbons and static runes

    private static final Logger LOGGER = LoggerFactory.getLogger(GameManagerService.class);
    private static final String OS = System.getProperty("os.name", "generic").toLowerCase(Locale.ENGLISH);
    private final TaskScheduler taskScheduler;
    private final List<Integer> serverList = new ArrayList<>();
    private final Map<ServerKillTask, ScheduledFuture<?>> tasks = new ConcurrentHashMap<>();
    @Value("${game.server.path}")
    private String gameServerPath;
    @Value("${game.server.maxgametime}")
    private int maxGameTime;
    @Value("${game.server.ip}")
    private String gameServerIp;

    public GameManagerService(TaskScheduler taskScheduler) {
        this.taskScheduler = taskScheduler;
    }

    @PostConstruct
    public void fill() {
        for (var i = 1337; i <= 1348; i++) {
            serverList.add(i);
        }
    }

    @Scheduled(initialDelay = 5000, fixedDelay = 10000)
    private void recheckProcesses() {
        List<ServerKillTask> toRemove = new ArrayList<>();
        for (Map.Entry<ServerKillTask, ScheduledFuture<?>> entry : tasks.entrySet()) {
            if (!entry.getKey().getProcess().isAlive()) {
                entry.getValue().cancel(true);

                if (!serverList.contains(entry.getKey().getToAdd())) {
                    serverList.add(entry.getKey().getToAdd());
                }

                toRemove.add(entry.getKey());
            }
        }

        toRemove.forEach(tasks.keySet()::remove);
    }

    public List<GameStartOut> start(GameLobby gameLobby) {
        List<GameStartOut> gameStartOuts = new ArrayList<>();
        var objectMapper = new ObjectMapper();
        var objectWriter = objectMapper.writer(new DefaultPrettyPrinter());
        var config = getGameConfig(gameLobby);
        var settings = getGameSettings();
        var port = serverList.isEmpty() ? null : serverList.get(0);

        if (config == null || port == null) {
            // This happens when there is a problem with the path-finding and it will be already logged.
            return Collections.emptyList();
        }

//        try (var s = new ServerSocket(0)) {
//            port = s.getLocalPort();
//        } catch (IOException e) {
//            LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
//            return Collections.emptyList();
//        }

        var settingsDir = new File(gameServerPath + "/Settings");

        // This checks if the directory doesn't exist yet and if it doesn't exists if it could be created
        if (settingsDir.isDirectory() || settingsDir.mkdir()) {
            try {
                objectWriter.writeValue(new File(gameServerPath + "/Settings/GameInfo.json"), config);
                objectWriter.writeValue(new File(gameServerPath + "/Settings/GameServerSettings.json"), settings);
            } catch (IOException e) {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
                return Collections.emptyList();
            }
        }

        try {
            /*
            This is a fun one.

            Starting the GameServer without cmd /c results it in being a subprocess to the java process.
            This would be fine, but the GameServer is using blocking stdout (text in console) which the java process is the parent of and needs to handle.
            So if we're not handling it, the buffer of those streams fill up and the GameServer will be 'blocked'.
            To fix this, we can detach the process with cmd /c and hide it with <nul 2<&1 or we need to empty the streams continually.

            We need to check the platform, because on Linux we cannot use cmd /c, but we can use >/dev/null 2>&1 there.
             */

            Process p;
            ProcessBuilder pB;
            if (OS.contains("win")) {
                pB = new ProcessBuilder(gameServerPath + "/GameServerConsole.exe", "--port", String.valueOf(port));
            } else {
                pB = new ProcessBuilder(gameServerPath + "/GameServerConsole", "--port", String.valueOf(port));
            }

            pB.directory(new File(gameServerPath));
            pB.redirectErrorStream(true);

            var logDir = new File(System.getProperty("user.dir") + "/logs");

            // This checks if the directory doesn't exist yet and if it doesn't exists if it could be created
            if (logDir.isDirectory() || logDir.mkdir()) {
                var log = new File(logDir + "/ls-gameserver_" + LocalDate.now() + ".log");

                pB.redirectOutput(ProcessBuilder.Redirect.appendTo(log));
                p = pB.start();

                serverList.remove(0);
                var serverKillTask = new ServerKillTask(p, serverList, port);
                tasks.put(serverKillTask, taskScheduler.schedule(serverKillTask, Instant.now().plusSeconds(maxGameTime * 60L)));
            } else {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION + " EXCEPTION: Log directory for LS server could not be created.");
            }
        } catch (IOException e) {
            LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
            return Collections.emptyList();
        }

        for (Player player : config.getPlayers()) {
            // Skip bot players — they don't connect to the server (controlled by ChampionAI script)
            if (player.getPlayerId() == -1) {
                continue;
            }
            var gameStartOut = new GameStartOut();
            gameStartOut.setIp(gameServerIp);
            gameStartOut.setPort(port);
            gameStartOut.setBlowfish(player.getBlowfishKey());
            gameStartOut.setPlayerId(player.getPlayerId());

            gameStartOuts.add(gameStartOut);
        }

        LOGGER.info(LogMessage.CHAMPSELECT_GAME_STARTED, gameLobby.getUuid(), port, maxGameTime);

        return gameStartOuts;
    }

    public List<Integer> getServerList() {
        return serverList;
    }

    private Config getGameConfig(GameLobby gameLobby) {
        var config = new Config();
        var game = new Game();
        var gameInfo = new GameInfo();
        List<Player> players = new ArrayList<>();

        game.setMap(gameLobby.getLobbyType().getMapType());
        game.setDataPackage("LeagueSandbox-Scripts");

        // Set to ARAM if map is ARAM
        if (gameLobby.getLobbyType().getMapType() == 12) {
            game.setGameMode("ARAM");
        } else {
            game.setGameMode("CLASSIC");
        }

        String contentPath = getContentPath();

        if (contentPath == null) {
            return null;
        }

        gameInfo.setManaCostsEnabled(true);
        gameInfo.setCooldownsEnabled(true);
        gameInfo.setCheatsEnabled(false);
        gameInfo.setMinionSpawnsEnabled(true);
        gameInfo.setContentPath(contentPath);
        gameInfo.setDamageTextGlobal(false);

        for (var i = 0; i < gameLobby.getAllUsers().size(); i++) {
            var lobbyUser = gameLobby.getAllUsers().get(i);
            var p = new Player();
            p.setPlayerId(i + 1);
            p.setBlowfishKey("17BLOhi6KZsTtldTsizvHg==");
            p.setRank("DIAMOND");
            p.setName(lobbyUser.getUser().getSummonerName());
            p.setChampion(lobbyUser.getSelectedChampion().getId());
            p.setTeam(lobbyUser.getTeam() == LobbyTeam.TEAM1 ? "BLUE" : "RED");
            if (lobbyUser.getSelectedSkin() != null) {
                p.setSkin(lobbyUser.getSelectedSkin().getPictureId());
            }
            p.setSummoner1(lobbyUser.getSpell1().getGameServerName());
            p.setSummoner2(lobbyUser.getSpell2().getGameServerName());
            p.setRibbon(2);
            p.setIcon(0);
            p.setRunes(lobbyUser.getRunes() != null ? lobbyUser.getRunes() : new Runes());
            p.setTalents(lobbyUser.getTalents() != null ? lobbyUser.getTalents() : new Talents());
            players.add(p);
        }

        // Inject bot players with playerId = -1 to activate ChampionAI script.
        // Bot name format: {Team}{Role} (e.g., BlueTop) — used by ChampionAI for role detection.
        for (var bot : gameLobby.getBots()) {
            var p = new Player();
            p.setPlayerId(-1);
            p.setBlowfishKey("17BLOhi6KZsTtldTsizvHg==");
            p.setRank("DIAMOND");
            p.setName(bot.getName());
            p.setChampion(bot.getChampionId());
            p.setTeam(bot.getTeam() == LobbyTeam.TEAM1 ? "BLUE" : "RED");
            p.setSkin(0);
            p.setSummoner1(getSummoner1ForRole(bot.getName()));
            p.setSummoner2(SummonerSpell.SUMMONER_FLASH.getGameServerName());
            p.setRibbon(2);
            p.setIcon(0);
            p.setRunes(getRunesForRole(bot.getName()));
            p.setTalents(getTalentsForRole(bot.getName()));
            players.add(p);
        }

        config.setGame(game);
        config.setGameInfo(gameInfo);
        config.setPlayers(players);

        return config;
    }

    private Runes getRunesForRole(String name) {
        Runes r = new Runes();
        if (name == null) {
            return r;
        }

        String role = detectRole(name);
        switch (role) {
            case "Mid":
                r.setOne(Runes.MARK_AP); r.setTwo(Runes.MARK_AP); r.setThree(Runes.MARK_AP);
                r.setFour(Runes.MARK_AP); r.setFive(Runes.MARK_AP); r.setSix(Runes.MARK_AP);
                r.setSeven(Runes.MARK_AP); r.setEight(Runes.MARK_AP); r.setNine(Runes.MARK_AP);
                r.setTen(Runes.SEAL_MANA_REGEN); r.setEleven(Runes.SEAL_MANA_REGEN); r.setTwelve(Runes.SEAL_MANA_REGEN);
                r.setThirteen(Runes.SEAL_MANA_REGEN); r.setFourteen(Runes.SEAL_MANA_REGEN); r.setFifteen(Runes.SEAL_MANA_REGEN);
                r.setSixteen(Runes.SEAL_MANA_REGEN); r.setSeventeen(Runes.SEAL_MANA_REGEN); r.setEighteen(Runes.SEAL_MANA_REGEN);
                r.setNineteen(Runes.GLYPH_AP); r.setTwenty(Runes.GLYPH_AP); r.setTwentyOne(Runes.GLYPH_AP);
                r.setTwentyTwo(Runes.GLYPH_AP); r.setTwentyThree(Runes.GLYPH_AP); r.setTwentyFour(Runes.GLYPH_AP);
                r.setTwentyFive(Runes.GLYPH_AP); r.setTwentySix(Runes.GLYPH_AP); r.setTwentySeven(Runes.GLYPH_AP);
                r.setTwentyEight(Runes.QUINTESSENCE_AP); r.setTwentyNine(Runes.QUINTESSENCE_AP); r.setThirty(Runes.QUINTESSENCE_AP);
                break;
            case "Support":
                r.setOne(Runes.MARK_ARMOR); r.setTwo(Runes.MARK_ARMOR); r.setThree(Runes.MARK_ARMOR);
                r.setFour(Runes.MARK_ARMOR); r.setFive(Runes.MARK_ARMOR); r.setSix(Runes.MARK_ARMOR);
                r.setSeven(Runes.MARK_ARMOR); r.setEight(Runes.MARK_ARMOR); r.setNine(Runes.MARK_ARMOR);
                r.setTen(Runes.SEAL_HEALTH); r.setEleven(Runes.SEAL_HEALTH); r.setTwelve(Runes.SEAL_HEALTH);
                r.setThirteen(Runes.SEAL_HEALTH); r.setFourteen(Runes.SEAL_HEALTH); r.setFifteen(Runes.SEAL_HEALTH);
                r.setSixteen(Runes.SEAL_HEALTH); r.setSeventeen(Runes.SEAL_HEALTH); r.setEighteen(Runes.SEAL_HEALTH);
                r.setNineteen(Runes.GLYPH_MANA_REGEN); r.setTwenty(Runes.GLYPH_MANA_REGEN); r.setTwentyOne(Runes.GLYPH_MANA_REGEN);
                r.setTwentyTwo(Runes.GLYPH_MANA_REGEN); r.setTwentyThree(Runes.GLYPH_MANA_REGEN); r.setTwentyFour(Runes.GLYPH_MANA_REGEN);
                r.setTwentyFive(Runes.GLYPH_MANA_REGEN); r.setTwentySix(Runes.GLYPH_MANA_REGEN); r.setTwentySeven(Runes.GLYPH_MANA_REGEN);
                r.setTwentyEight(Runes.QUINTESSENCE_MOVEMENT_SPEED); r.setTwentyNine(Runes.QUINTESSENCE_MOVEMENT_SPEED); r.setThirty(Runes.QUINTESSENCE_MOVEMENT_SPEED);
                break;
            case "ADC":
                r.setOne(Runes.MARK_AD); r.setTwo(Runes.MARK_AD); r.setThree(Runes.MARK_AD);
                r.setFour(Runes.MARK_AD); r.setFive(Runes.MARK_AD); r.setSix(Runes.MARK_AD);
                r.setSeven(Runes.MARK_AD); r.setEight(Runes.MARK_AD); r.setNine(Runes.MARK_AD);
                r.setTen(Runes.SEAL_ARMOR); r.setEleven(Runes.SEAL_ARMOR); r.setTwelve(Runes.SEAL_ARMOR);
                r.setThirteen(Runes.SEAL_ARMOR); r.setFourteen(Runes.SEAL_ARMOR); r.setFifteen(Runes.SEAL_ARMOR);
                r.setSixteen(Runes.SEAL_ARMOR); r.setSeventeen(Runes.SEAL_ARMOR); r.setEighteen(Runes.SEAL_ARMOR);
                r.setNineteen(Runes.GLYPH_ATTACK_SPEED); r.setTwenty(Runes.GLYPH_ATTACK_SPEED); r.setTwentyOne(Runes.GLYPH_ATTACK_SPEED);
                r.setTwentyTwo(Runes.GLYPH_ATTACK_SPEED); r.setTwentyThree(Runes.GLYPH_ATTACK_SPEED); r.setTwentyFour(Runes.GLYPH_ATTACK_SPEED);
                r.setTwentyFive(Runes.GLYPH_ATTACK_SPEED); r.setTwentySix(Runes.GLYPH_ATTACK_SPEED); r.setTwentySeven(Runes.GLYPH_ATTACK_SPEED);
                r.setTwentyEight(Runes.QUINTESSENCE_ATTACK_SPEED); r.setTwentyNine(Runes.QUINTESSENCE_ATTACK_SPEED); r.setThirty(Runes.QUINTESSENCE_LIFESTEAL);
                break;
            case "Jungle":
                r.setOne(Runes.MARK_AD); r.setTwo(Runes.MARK_AD); r.setThree(Runes.MARK_AD);
                r.setFour(Runes.MARK_AD); r.setFive(Runes.MARK_AD); r.setSix(Runes.MARK_AD);
                r.setSeven(Runes.MARK_AD); r.setEight(Runes.MARK_AD); r.setNine(Runes.MARK_AD);
                r.setTen(Runes.SEAL_ARMOR); r.setEleven(Runes.SEAL_ARMOR); r.setTwelve(Runes.SEAL_ARMOR);
                r.setThirteen(Runes.SEAL_ARMOR); r.setFourteen(Runes.SEAL_ARMOR); r.setFifteen(Runes.SEAL_ARMOR);
                r.setSixteen(Runes.SEAL_ARMOR); r.setSeventeen(Runes.SEAL_ARMOR); r.setEighteen(Runes.SEAL_ARMOR);
                r.setNineteen(Runes.GLYPH_MAGIC_RESIST); r.setTwenty(Runes.GLYPH_MAGIC_RESIST); r.setTwentyOne(Runes.GLYPH_MAGIC_RESIST);
                r.setTwentyTwo(Runes.GLYPH_MAGIC_RESIST); r.setTwentyThree(Runes.GLYPH_MAGIC_RESIST); r.setTwentyFour(Runes.GLYPH_MAGIC_RESIST);
                r.setTwentyFive(Runes.GLYPH_MAGIC_RESIST); r.setTwentySix(Runes.GLYPH_MAGIC_RESIST); r.setTwentySeven(Runes.GLYPH_MAGIC_RESIST);
                r.setTwentyEight(Runes.QUINTESSENCE_AD); r.setTwentyNine(Runes.QUINTESSENCE_AD); r.setThirty(Runes.QUINTESSENCE_HP_REGEN);
                break;
            case "Top":
            default:
                r.setOne(Runes.MARK_AD); r.setTwo(Runes.MARK_AD); r.setThree(Runes.MARK_AD);
                r.setFour(Runes.MARK_AD); r.setFive(Runes.MARK_AD); r.setSix(Runes.MARK_AD);
                r.setSeven(Runes.MARK_AD); r.setEight(Runes.MARK_AD); r.setNine(Runes.MARK_AD);
                r.setTen(Runes.SEAL_ARMOR); r.setEleven(Runes.SEAL_ARMOR); r.setTwelve(Runes.SEAL_ARMOR);
                r.setThirteen(Runes.SEAL_ARMOR); r.setFourteen(Runes.SEAL_ARMOR); r.setFifteen(Runes.SEAL_ARMOR);
                r.setSixteen(Runes.SEAL_ARMOR); r.setSeventeen(Runes.SEAL_ARMOR); r.setEighteen(Runes.SEAL_ARMOR);
                r.setNineteen(Runes.GLYPH_MAGIC_RESIST); r.setTwenty(Runes.GLYPH_MAGIC_RESIST); r.setTwentyOne(Runes.GLYPH_MAGIC_RESIST);
                r.setTwentyTwo(Runes.GLYPH_MAGIC_RESIST); r.setTwentyThree(Runes.GLYPH_MAGIC_RESIST); r.setTwentyFour(Runes.GLYPH_MAGIC_RESIST);
                r.setTwentyFive(Runes.GLYPH_MAGIC_RESIST); r.setTwentySix(Runes.GLYPH_MAGIC_RESIST); r.setTwentySeven(Runes.GLYPH_MAGIC_RESIST);
                r.setTwentyEight(Runes.QUINTESSENCE_HP); r.setTwentyNine(Runes.QUINTESSENCE_HP); r.setThirty(Runes.QUINTESSENCE_HP);
                break;
        }
        return r;
    }

    private String getSummoner1ForRole(String name) {
        if (name == null) {
            return SummonerSpell.SUMMONER_FLASH.getGameServerName();
        }
        if (name.contains("Jungle")) {
            return SummonerSpell.SUMMONER_SMITE.getGameServerName();
        } else if (name.contains("Support")) {
            return SummonerSpell.SUMMONER_EXHAUST.getGameServerName();
        } else if (name.contains("ADC")) {
            return SummonerSpell.SUMMONER_HEAL.getGameServerName();
        } else if (name.contains("Mid")) {
            return SummonerSpell.SUMMONER_DOT.getGameServerName();
        }
        return SummonerSpell.SUMMONER_TELEPORT.getGameServerName();
    }

    private Talents getTalentsForRole(String name) {
        Talents t = new Talents();
        if (name == null) {
            return t;
        }

        String role = detectRole(name);
        switch (role) {
            case "Mid":
                t.setBrutalForce(0);
                t.setSorcery(3);
                t.setFeast(0);
                t.setVeteranScars(0);
                t.setMeditation(3);
                t.setWrath(0);
                t.setArcaneKnowledge(3);
                t.setSpellWeaving(1);
                t.setSpellPiercing(3);
                t.setArchmage(1);
                t.setThunderlordsDecree(1);
                break;
            case "Support":
                t.setBrutalForce(0);
                t.setSorcery(0);
                t.setFeast(3);
                t.setVeteranScars(3);
                t.setMeditation(3);
                t.setWrath(0);
                t.setIntelligence(3);
                t.setHealingMastery(3);
                t.setNaturalTalent(3);
                t.setJuggernaut(3);
                t.setTenacity(1);
                t.setGraspOfTheUndying(1);
                break;
            case "ADC":
                t.setBrutalForce(3);
                t.setFury(3);
                t.setFeast(3);
                t.setVeteranScars(0);
                t.setMeditation(0);
                t.setVampirism(3);
                t.setWrath(3);
                t.setPrecision(3);
                t.setDoubleEdgedSword(1);
                t.setExecutioner(3);
                t.setFervorOfBattle(1);
                break;
            case "Jungle":
                t.setBrutalForce(3);
                t.setFury(3);
                t.setFeast(3);
                t.setVeteranScars(3);
                t.setMeditation(3);
                t.setWrath(3);
                t.setPrecision(3);
                t.setDoubleEdgedSword(1);
                t.setExecutioner(3);
                t.setAssassin(1);
                t.setFervorOfBattle(1);
                t.setJuggernaut(3);
                break;
            case "Top":
            default:
                t.setBrutalForce(3);
                t.setFury(3);
                t.setFeast(3);
                t.setVeteranScars(3);
                t.setMeditation(3);
                t.setWrath(3);
                t.setPrecision(3);
                t.setExecutioner(3);
                t.setJuggernaut(3);
                t.setDoubleEdgedSword(1);
                t.setFervorOfBattle(1);
                break;
        }
        return t;
    }

    private String detectRole(String name) {
        if (name == null) {
            return "Unknown";
        }

        String normalized = name.toLowerCase(Locale.ROOT);
        if (normalized.contains("mid")) {
            return "Mid";
        }
        if (normalized.contains("support")) {
            return "Support";
        }
        if (normalized.contains("adc")) {
            return "ADC";
        }
        if (normalized.contains("jungle")) {
            return "Jungle";
        }
        if (normalized.contains("top")) {
            return "Top";
        }
        return "Unknown";
    }

    private Settings getGameSettings() {
        return new Settings(false);
    }

    private String getContentPath() {
        String result = null;
        var path = Path.of(gameServerPath);

        while (result == null) {
            if (path == null) {
                break;
            }

            try (Stream<Path> s = Files.walk(path, 1)) {
                List<Path> paths = s.filter(c -> c.toFile().isDirectory() && c.toFile().getName().equals("Content")).collect(Collectors.toList());

                if (paths.size() == 1) {
                    result = paths.get(0).toString();
                } else {
                    path = path.getParent();
                }
            } catch (IOException e) {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
                return null;
            }
        }
        return result;
    }

}
