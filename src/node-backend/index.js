const ws = require("nodejs-websocket");
const gameLogic = require('./gamelogic');

let users = [];

function sendToAll(data, exclude) {
    var jsondata = JSON.stringify(data);
    wsserver.connections.forEach(function (conn) {
        if (conn != exclude) {
            conn.sendText(jsondata);
        }
    });
}

var netgame = new gameLogic();

var wsserver = ws.createServer(function (conn) {
    var user = {
        id: conn.key,
        nr: netgame.addUser(),
        color: users.length + 1
    };
    users.push(user);
    if (!currentUser)
        currentUser = conn.key;

    var initData = JSON.stringify({
        user: user,
        points: grid
    });

    conn.sendText(initData);

    conn.on('text', function (data) {
        var obj = JSON.parse(data);
        console.log(obj);
        if (obj) {

        }
    });
    
    conn.on('close', function () {
        netgame.removeUser(user.nr);
    });
});