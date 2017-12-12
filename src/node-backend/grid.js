const pos = require('./pos');

const grid = function () {
    var t = this;
    this.points = [];

    function getPoint(p) {
        var val = new pos(p.x,p.y,0);
        t.points.map(function (v,i) {
            if (v.match(p)) {
                val = v;
                val.idx = i;
            }
        });
        return val;
    }

    function getValue(pos) {
        var val = getPoint(pos);
        return val.value;
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

    this.clear = function(list) {
        if (!list) {
            t.points = [];
        }
        else {
            list.map(function(v){
                var op = getPoint(v);
                if (op.idx) {
                    t.points.splice(op.idx,1);
                }
            });
        }
    }

    this.removeWithValue = function (nr) {
        var newlist = t.points.map(function (v) {
            if (v.value == nr)
                v.value = 0;
            return v;
        });
        t.points = newlist;
    }

    this.getArray = function () {
        return t.points.map(function (v) {
            return {
                x: v.x,
                y: v.y,
                v: v.value
            };
        });
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

module.exports = grid;