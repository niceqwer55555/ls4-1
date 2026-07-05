var child_process = require('child_process');
var LobbyManagerService = require('../services/LobbyManagerService.js');

function LobbyFactory(){

  return {
    createLobby: createLobby
  };
  function createLobby(options, port, configPath, GameServerPort){
    var lobby;

    lobby = {
        name: options.name,
        creator: options.creator,
        playerLimit: options.playerLimit,
        playerCount: 0,
        gameMode: options.gamemodeName,
        requirePassword: false,
        address: "http://localhost",
        port: port,
        lobbyType: options.lobbyType || "SUMMONERS_RIFT_BLIND"
    };
    module.exports = {
        name: options.name,
        creator: options.creator,
        playerLimit: options.playerLimit,
        playerCount: 0,
        gameMode: options.gamemodeName,
        requirePassword: false,
        address: "http://localhost",
        port: port,
        lobbyType: options.lobbyType || "SUMMONERS_RIFT_BLIND"
    };
    var fork = require('child_process').fork;
    var child = fork('lobby', [port, configPath, GameServerPort, options.lobbyType || "SUMMONERS_RIFT_BLIND"]);
    return lobby;
  }
}
module.exports = LobbyFactory();
