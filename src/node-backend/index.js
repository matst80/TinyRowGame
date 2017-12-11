var ws = require("nodejs-websocket");

let users = [];

function getNextUser(userId) {
    var next;
    var ret;
    users.map(function (v, i) {
        if (next) {
            ret = v;
            return false;
        }
        if (v.id == userId)
            next = true;
    });
    return ret;
}

/*

    0  | 
    1  /
    2  - 
    3  \ 
    4  |
    5  /
    6  -
    7  \

*/

const pos = function (x, y, val) {
    var t = this;

    this.x = x;
    this.y = y;
    this.value = val;

    this.match = function (pos) {
        return (pos.x == t.x && pos.y == t.y);
    }

    this.direction = function (dir) {
        var diffX;
        var diffY;
        switch (dir) {
            case 0: // up |
                diffY = 1;
                diffX = 0;
                break;
            case 1: // up-right /
                diffY = 1;
                diffX = 1;
                break;
            case 2: // right -
                diffY = 0;
                diffX = 1;
                break;
            case 3: // down-right \
                diffY = -1;
                diffX = 1;
                break;
            case 4: // down |
                diffY = 0;
                diffX = -1;
                break;
            case 5: // down-left /
                diffY = -1;
                diffX = -1;
                break;
            case 6: // left /
                diffY = 0;
                diffX = -1;
                break;
            case 7: // up-left \
                diffY = -1;
                diffX = -1;
                break;
        }
        return new pos(t.x + diffX, t.y + diffY, t.val);
    }
}

const grid = function (winLength) {
    var t = this;
    this.pointInRowToWin = winLength;
    this.points = [];

    function getValue(pos) {
        var val = 0;
        t.points.map(function (v) {
            if (v.match(pos))
                val = v.value; 
        });
        return val;
    }

    function isFree(pos) {
        return getValue(pos)==0;
    }

    this.addPoint = function (pos) {
        var isValid = isFree(pos);
        if (isValid) {
            t.points.push(pos);
            var inRow = t.checkFromPoint(pos);
            //console.log('maxrow',inRow);            
            return inRow;
        }
        else
            return -1;
    }

    this.checkFromPoint = function (p) {
        var winArr = [];
        var winValue = p.value;
        console.log(t.points);
        for(var i=0;i<7;i++) {
            var dirPos = new pos(p.x,p.y);
            var dirArr = [];            
            for(var j=0;j<t.pointInRowToWin;j++) {
                if (getValue(dirPos) == winValue) {
                    dirPos = dirPos.direction(i);
                    winArr.push(dirPos);
                }
                else {
                    break;
                }
            }
            if (dirArr.length>winArr.length)
                winArr = dirArr;
        }
        return winArr;
    }
}

const gameLogic = function () {

    var t = this;

    function getNextUser() {
        var idx = t.users.indexOf(t.currentUser) + 1;
        if (idx >= t.users.length)
            idx = 0;
        return t.users[idx];
    }

    this.reset = function () {
        this.grid = new grid();
        this.users = [];
        this.currentUser = false;
    }

    this.reset();

    this.addUser = function () {
        var userNr = t.users.length + 1;
        if (!t.currentUser)
            t.currentUser = userNr;
        t.users.push(userNr);
        return userNr;
    }

    this.handleMove = function (userId, move) {
        console.log(userId,t.currentUser);
        if (userId == t.currentUser) {
            t.currentUser = getNextUser(userId);
            return t.grid.addPoint(new pos(move.x, move.y, userId));
        }
        else
            return -1;
    }
}

var game = new gameLogic();
var usrId = game.addUser();

for (var i = 0; i < 6; i++) {
    console.log('add',i,game.handleMove(usrId, { x: i, y: 1 }));
}

return;

function sendToAll(data, exclude) {
    var jsondata = JSON.stringify(data);
    wsserver.connections.forEach(function (conn) {
        if (conn != exclude) {
            conn.sendText(jsondata);
        }
    });
}

var wsserver = ws.createServer(function (conn) {
    var user = {
        id: conn.key,
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

    });
});