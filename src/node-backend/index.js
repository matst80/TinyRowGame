const ws = require("nodejs-websocket");
const gameLogic = require('./gamelogic');
const extend = require('node.extend');

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
    console.log('newuser');
    var listensers = {
        "maxlength": function (points) {
            send('maxlen', { connected: points });
        },
        "grid": function (points) {
            send('grid', { grid: points });
        },
        "userlist": function (users) {
            send('userlist', { users: users });
        },
        "winner": function (userNr) {
            send('winner', { winner: userNr });
        },
        "turn": function (turn) {
            if (turn == user.nr) {
                send('turn', {});
            }
        }
    };

    for (var prp in listensers) {
        netgame.emitter.addListener(prp, listensers[prp]);
    }

    var user = {
        id: conn.key,
        nr: netgame.addUser(),
        color: users.length + 1
    };

    function send(type, jsonObj) {
        console.log('in send');
        if (!type || !jsonObj)
            return;
        var obj = extend({ type: type }, jsonObj);

        if (conn && conn.readyState == 1) {
            console.log('SEND:', obj);
            conn.sendText(JSON.stringify(obj));
        }
    }

    users.push(user);

    send('init', {
        user: user,
        points: netgame.grid.getArray()
    });

    conn.on('text', function (data) {
        var obj = JSON.parse(data);
        console.log('GOT', obj);
        if (obj) {
            switch (obj.type) {
                case 'place':
                    netgame.handleMove(user.nr, obj);
                    break;

                default:
                    break;
            }
        }
    });

    conn.on('error', function (err) {
        console.log(err);
        netgame.removeUser(user.nr);
        for (var prp in listensers) {
            netgame.emitter.removeListener(prp, listensers[prp]);
        }
    });
    conn.on('close', function () {
        console.log('close');
        netgame.removeUser(user.nr);
        for (var prp in listensers) {
            netgame.emitter.removeListener(prp, listensers[prp]);
        }
    });

}).listen(process.env.PORT || 8001);