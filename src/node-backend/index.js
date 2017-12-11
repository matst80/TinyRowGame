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

const pos = function (x, y, val) {
    var t = this;

    this.x = x;
    this.y = y;
    this.value = val;

    this.match = function (pos) {
        return (pos.x === t.x && pos.y === t.y);
    }
}

pos.prototype.direction = function (dir) {
    var t = this;
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
            diffY = -1;
            diffX = 0;
            break;
        case 5: // down-left /
            diffY = -1;
            diffX = -1;
            break;
        case 6: // left -
            diffY = 0;
            diffX = -1;
            break;
        case 7: // up-left \
            diffY = -1;
            diffX = -1;
            break;
    }
    return new pos(t.x + diffX, t.y + diffY, t.value);
};

pos.prototype.directionInvert = function(dir) {
    return this.direction(dir+4);
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
        return getValue(pos) == 0;
    }

    this.addPoint = function (pos) {
        var isValid = isFree(pos) && pos.value;
        if (isValid) {
            t.points.push(pos);
            return t.checkFromPoint(pos);
        }
        else
            return -1;
    }

    this.checkFromPoint = function (p) {
        let winArr = [];
        let winValue = p.value;
        for (var i = 0; i < 4; i++) {
            let dirPos = new pos(p.x, p.y, p.value);
            let dirNeg = p.directionInvert(i);
            let dirArr = [];
            while (getValue(dirPos) == winValue) {
                dirArr.push(dirPos);
                dirPos = dirPos.direction(i);
            }
            while (getValue(dirNeg) == winValue) {
                dirArr.push(dirNeg);
                dirNeg = dirNeg.directionInvert(i);
            }
            
            if (dirArr.length > winArr.length) {
                winArr = [];
                winArr = dirArr;
            }
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

var tmpgrid = [];
for(var y=0;y<15;y++) {
    var row = [];
    tmpgrid.push(row);
    for(var x=0;x<15;x++) {
        row.push(0);
    }
}

var maxArr = [];

for (var i = 0; i < 100; i++) {
    var x = Math.round(Math.random()*14);
    var y = Math.round(Math.random()*14);
    //console.log(x,y);
    tmpgrid[y][x] = usrId;
    var ret = game.handleMove(usrId, { x: x, y: y });
    if (ret.length)
    {
        if (ret.length>maxArr.length) {
            console.log('record',ret);
            maxArr = [];
            maxArr = ret;
        }
    }
    else {
        
    }
}

tmpgrid.map(function(row) {
    console.log(row.join(' '));
});

console.log('maxrow',maxArr);

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