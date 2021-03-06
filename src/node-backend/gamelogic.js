const pos = require('./pos');
const grid = require('./grid');
const EventEmitter = require('events');

class GameEvents extends EventEmitter {

}

var totusers = 0;

const gameLogic = function (opt) {

    this.emitter = new GameEvents();
    this.settings = opt||{};
    this.pointInRowToWin = this.settings.winningLength||5;


    var t = this;

    function setNextUser() {
        if (t.users.length == 0) {
            t.reset();
            return;
        }
        var idx = t.users.indexOf(t.currentUser) + 1;
        if (idx >= t.users.length)
            idx = 0;
        t.currentUser = t.users[idx];
        t.emitter.emit('turn', t.currentUser);
    }

    this.on = this.emitter.on;

    this.reset = function () {
        t.grid = new grid();
        t.users = [];
        t.currentUser = false;
        t.emitter.emit('reset', t.grid);
    }

    this.reset();

    this.addUser = function () {
        var userNr = ++totusers;
        if (!t.currentUser)
            t.currentUser = userNr;
        t.users.push(userNr);

        t.emitter.emit('adduser', userNr);
        t.emitter.emit('userlist', t.users);

        return userNr;
    }

    this.removeUser = function (nr) {
        var idx = t.users.indexOf(nr);
        if (idx !== -1) {
            t.users.splice(idx, 1);
            t.grid.removeWithValue(nr);
        }
        if (t.users.length == 0) {
            t.reset();
        }
        else {
            t.emitter.emit('removeuser', nr);
            t.emitter.emit('userlist', t.users);
            t.emitter.emit('grid', t.grid.getArray());

            if (t.currentUser == nr) {
                setNextUser();
            }
        }
    }

    this.handleMove = function (userId, move) {
        if (userId == t.currentUser) {
            var ret = t.grid.addPoint(new pos(move.x, move.y, userId));
            if (ret != -1) {
                if (ret.length >= this.pointInRowToWin) {
                    t.emitter.emit('winner', userId);
                    t.grid.clear(ret);
                }
                setNextUser();
                t.emitter.emit('maxlength', ret, userId);
                t.emitter.emit('grid', t.grid.getArray());
            }
            return ret;
        }
        else
            return -1;
    }
}

module.exports = gameLogic;