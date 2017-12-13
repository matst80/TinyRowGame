const pos = function (x, y, val) {
    var t = this;

    this.x = x;
    this.y = y;
    this.value = val;
    this.id = posIdCount++;

    this.match = function (pos) {
        return (pos.x === t.x && pos.y === t.y);
    }
}

var posIdCount = 0;

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

module.exports = pos;