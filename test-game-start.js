const WebSocket = require('D:/game/ls4-1/ls4-launcher/node_modules/ws');
const axios = require('D:/game/ls4-1/ls4-launcher/node_modules/axios').default;

async function test() {
  try {
    console.log('Step 1: Login');
    const loginRes = await axios.post('http://127.0.0.1:8080/users/login', {
      userName: 'testuser', password: 'Test123456'
    });
    const token = loginRes.data.token;
    console.log('Login OK');

    const ws = new WebSocket('ws://127.0.0.1:8080/socket?token=' + token);
    
    ws.on('open', function() {
      console.log('WS connected');
      ws.send('CONNECT\naccept-version:1.2\nheart-beat:10000,10000\n\n\x00');
    });
    
    ws.on('message', function(data) {
      const frame = data.toString();
      
      if (frame.startsWith('CONNECTED')) {
        console.log('STOMP CONNECTED');
        ws.send('SUBSCRIBE\nid:sub-0\ndestination:/user/queue/system\n\n\x00');
        
        setTimeout(function() {
          console.log('Step 3: Create Lobby');
          ws.send('SEND\ndestination:/out/system\ncontent-type:application/json\n\n' + JSON.stringify({id:'t1',messageType:'LOBBY_CREATE',data:{lobbyType:'SUMMONERS_RIFT_BOT_BLIND',isCustom:true}}) + '\x00');
        }, 1500);
      }
      
      if (frame.includes('MESSAGE')) {
        const bi = frame.indexOf('\n\n');
        if (bi > -1) {
          const body = frame.substring(bi + 2).replace(/\x00/g, '');
          try {
            const msg = JSON.parse(body);
            console.log('MSG:', msg.messageType, msg.error ? 'ERR:' + JSON.stringify(msg.error) : '');
            
            if (msg.messageType === 'LOBBY_CREATE' && msg.data && !msg.error) {
              console.log('Lobby created:', msg.data.uuid);
              ws.send('SUBSCRIBE\nid:sub-lobby\ndestination:/user/queue/lobby' + msg.data.uuid + '\n\n\x00');
              
              setTimeout(function() {
                console.log('Step 4: Add Bot');
                ws.send('SEND\ndestination:/out/lobby\ncontent-type:application/json\n\n' + JSON.stringify({id:'t2',messageType:'LOBBY_ADD_BOT',data:{championId:'KogMaw',difficulty:'NORMAL',team:'TEAM2',role:'Top'}}) + '\x00');
              }, 1500);
            }
            
            if (msg.messageType === 'LOBBY_ADD_BOT' && !msg.error) {
              console.log('Bot added!');
              
              setTimeout(function() {
                console.log('Step 5: Start Matchmaking');
                ws.send('SEND\ndestination:/out/lobby\ncontent-type:application/json\n\n' + JSON.stringify({id:'t3',messageType:'LOBBY_MATCHMAKING_START',data:{championId:'FiddleSticks',runePage:{"1":5245,"2":5245,"3":5245,"4":5245,"5":5245,"6":5245,"7":5245,"8":5245,"9":5245,"10":5317,"11":5317,"12":5317,"13":5317,"14":5317,"15":5317,"16":5317,"17":5317,"18":5317,"19":5289,"20":5289,"21":5289,"22":5289,"23":5289,"24":5289,"25":5289,"26":5289,"27":5289,"28":5335,"29":5335,"30":5335},masteryPage:{"4211":1,"4212":4,"4214":2,"4221":2,"4222":3,"4231":2,"4232":1,"4241":2,"4242":1,"4243":3,"4244":1,"4251":1,"4262":1,"4111":2,"4112":2,"4114":1,"4122":2,"4131":3}}}) + '\x00');
              }, 2000);
            }
            
            if (msg.messageType === 'LOBBY_CHAMPSELECT_SUBSCRIBE' && msg.data) {
              console.log('Champselect started:', msg.data.uuid);
              ws.send('SUBSCRIBE\nid:sub-cs\ndestination:/user/queue/champselect' + msg.data.uuid + '\n\n\x00');
              
              setTimeout(function() {
                console.log('Step 6: Lock Champion');
                ws.send('SEND\ndestination:/out/champselect\ncontent-type:application/json\n\n' + JSON.stringify({id:'t4',messageType:'CHAMPSELECT_LOCK_CHAMPION',data:{championId:'FiddleSticks',runes:{"1":5245,"2":5245,"3":5245,"4":5245,"5":5245,"6":5245,"7":5245,"8":5245,"9":5245,"10":5317,"11":5317,"12":5317,"13":5317,"14":5317,"15":5317,"16":5317,"17":5317,"18":5317,"19":5289,"20":5289,"21":5289,"22":5289,"23":5289,"24":5289,"25":5289,"26":5289,"27":5289,"28":5335,"29":5335,"30":5335},talents:{"4211":1,"4212":4,"4214":2,"4221":2,"4222":3,"4231":2,"4232":1,"4241":2,"4242":1,"4243":3,"4244":1,"4251":1,"4262":1,"4111":2,"4112":2,"4114":1,"4122":2,"4131":3}}}) + '\x00');
              }, 3000);
            }
            
            if (msg.messageType === 'CHAMPSELECT_GAME_START') {
              console.log('=== GAME START! ===');
              if (msg.data && msg.data.ip) {
                console.log('SUCCESS! IP=' + msg.data.ip + ' Port=' + msg.data.port + ' PlayerId=' + msg.data.playerId);
              } else {
                console.log('FAIL! Data=' + JSON.stringify(msg.data));
              }
              setTimeout(function() { ws.close(); process.exit(msg.data && msg.data.ip ? 0 : 1); }, 2000);
            }
            
            if (msg.messageType === 'CHAMPSELECT_UPDATE') {
              console.log('CS update...');
            }
            
          } catch(e) {}
        }
      }
    });
    
    ws.on('error', function(err) { console.error('WS error:', err.message); process.exit(1); });
    ws.on('close', function() { console.log('WS closed'); });
    setTimeout(function() { console.log('Timeout'); ws.close(); process.exit(1); }, 120000);
    
  } catch (err) {
    console.error('Error:', err.response ? JSON.stringify(err.response.data) : err.message);
    process.exit(1);
  }
}

test();
