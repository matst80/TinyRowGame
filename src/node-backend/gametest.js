const gameLogic = require('./gamelogic');

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

