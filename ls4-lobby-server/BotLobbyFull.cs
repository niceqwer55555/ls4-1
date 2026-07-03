using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum BotDifficulty
{
    Easy,
    Normal,
    Hard
}

public class Lobby
{
    public long Id { get; set; }
    public long OwnerId { get; set; }
    public string LobbyType { get; set; }
    public bool IsCustom { get; set; }
    public bool IsBotLobby { get; set; }
    public BotDifficulty Difficulty { get; set; }
    public List<LobbyMember> Members { get; set; } = new();
    public List<LobbyMember> Enemies { get; set; } = new();
}

public class LobbyMember
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public bool IsBot { get; set; }
    public int ChampionId { get; set; }
}

public class GameRoom
{
    public long GameId { get; set; }
    public long LobbyId { get; set; }
    public string Map { get; set; }
    public bool IsBotGame { get; set; }
    public BotDifficulty Difficulty { get; set; }
    public List<LobbyMember> BlueTeam { get; set; } = new();
    public List<LobbyMember> RedTeam { get; set; } = new();
}

public class User
{
    public long Id { get; set; }
    public string Username { get; set; }
}

public static class BotAiLogic
{
    public static (float gold, float exp, float skill, float agg) GetSettings(BotDifficulty diff)
    {
        return diff switch
        {
            BotDifficulty.Easy => (180, 160, 0.15f, 0.2f),
            BotDifficulty.Normal => (240, 220, 0.4f, 0.5f),
            BotDifficulty.Hard => (320, 300, 0.75f, 0.9f),
            _ => (200, 180, 0.3f, 0.4f)
        };
    }
}

public class BotLobbyService
{
    private readonly Random _rnd = new Random();

    public async Task<Lobby> CreateBotLobby(User creator, string lobbyType)
    {
        var diff = BotDifficulty.Normal;
        if (lobbyType.Contains("BOT_EASY")) diff = BotDifficulty.Easy;
        if (lobbyType.Contains("BOT_HARD")) diff = BotDifficulty.Hard;

        var lobby = new Lobby
        {
            Id = DateTime.UtcNow.Ticks % 10000000,
            OwnerId = creator.Id,
            LobbyType = lobbyType,
            IsBotLobby = true,
            Difficulty = diff
        };

        lobby.Members.Add(new LobbyMember
        {
            UserId = creator.Id,
            Username = creator.Username,
            IsBot = false
        });

        var roles = new[] { "上单", "打野", "中单", "AD", "辅助" };

        for (int i = 0; i < 4; i++)
        {
            lobby.Members.Add(new LobbyMember
            {
                UserId = -1000 - i,
                Username = $"{roles[i]}AI",
                IsBot = true,
                ChampionId = _rnd.Next(1, 150)
            });
        }

        for (int i = 0; i < 5; i++)
        {
            lobby.Enemies.Add(new LobbyMember
            {
                UserId = -2000 - i,
                Username = $"敌方{roles[i]}",
                IsBot = true,
                ChampionId = _rnd.Next(1, 150)
            });
        }

        return lobby;
    }
}

public class LobbyManager
{
    private readonly BotLobbyService _botLobbyService = new BotLobbyService();

    public async Task<Lobby> HandleCreateLobby(User user, string lobbyType, bool isCustom)
    {
        if (lobbyType.StartsWith("COOP_"))
        {
            return await _botLobbyService.CreateBotLobby(user, lobbyType);
        }

        return new Lobby
        {
            Id = DateTime.UtcNow.Ticks % 10000000,
            OwnerId = user.Id,
            LobbyType = lobbyType,
            IsCustom = isCustom,
            Members = new List<LobbyMember>
            {
                new LobbyMember { UserId = user.Id, Username = user.Username, IsBot = false }
            }
        };
    }

    public async Task<object> HandleStartGame(Lobby lobby, long userId)
    {
        if (lobby.OwnerId != userId)
            return new { success = false, message = "权限不足" };

        if (!lobby.IsBotLobby && lobby.Members.Count < 5)
            return new { success = false, message = "人数不足" };

        var game = new GameRoom
        {
            GameId = DateTime.UtcNow.Ticks % 9999999,
            LobbyId = lobby.Id,
            Map = lobby.LobbyType.Contains("ARAM") ? "ARAM" : "SUMMONERS_RIFT",
            IsBotGame = lobby.IsBotLobby,
            Difficulty = lobby.Difficulty,
            BlueTeam = lobby.Members,
            RedTeam = lobby.Enemies
        };

        return new { success = true, gameId = game.GameId, map = game.Map };
    }

    public async Task<object> CalculateReward(GameRoom game, bool win)
    {
        var (gold, exp, _, _) = BotAiLogic.GetSettings(game.Difficulty);
        var finalGold = win ? (int)(300 + gold * 0.2) : 100;
        var finalExp = win ? (int)(200 + exp * 0.2) : 60;

        return new
        {
            win,
            gold = finalGold,
            exp = finalExp,
            isBotGame = true
        };
    }
}