const pos = require('./pos');
const grid = require('./grid');
const EventEmitter = require('events');

class GameEvents extends EventEmitter {

}


const gameLogic = function (opt) {
    
    this.emitter = gameEvents = new GameEvents();
    this.settings = opt;

    var t = this;

    function setNextUser() {
        var idx = t.users.indexOf(t.currentUser) + 1;
        if (idx >= t.users.length)
            idx = 0;
        t.currentUser = t.users[idx];
        this.emitter.emit('turn', t.currentUser);
    }

    this.on = this.emitter.on;

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
        this.emitter.emit('adduser',userNr);
        this.emitter.emit('userlist',t.users);
        return userNr;
    }

    this.removeUser = function(nr) {
        var idx = t.users.indexOf(nr);
        if (idx !== -1) {
            t.users.splice(idx,1);
            t.grid.removeWithValue(nr);
        }
        this.emitter.emit('removeuser',userNr);
        this.emitter.emit('userlist',t.users);
        this.emitter.emit('grid',t.grid.getArray());
        if (t.currentUser==nr) {
            setNextUser();
        }
    }

    this.handleMove = function (userId, move) {
        if (userId == t.currentUser) {
            setNextUser();
            var ret = t.grid.addPoint(new pos(move.x, move.y, userId));
            this.emitter.emit('maxlength', ret, userId);
            this.emitter.emit('grid',t.grid.getArray());
        }
        else
            return -1;
    }
}

module.exports = gameLogic;