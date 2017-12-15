const TelnetServer = require('./src/telnetserver.js');
const GameClient = require('./src/gameclient.js');

const PORT = process.env.PORT || 23;
const GAMESERVER = process.env.SERVER || 'ws://localhost:8001';

function handler(conn) {
  console.log('Got new connection');

  var client = new GameClient(GAMESERVER);

  var col = 0;
  var row = 0;
  var player = 0;

  function redraw() {
    console.log('redraw, board', JSON.stringify(client.board));
    var buf = '';

    buf += '\x1b[2J\x1b[H';

    buf += 'Player ' + client.usernr + ' of ' + JSON.stringify(client.userlist);
    buf += ' - Cursor at [' + col + ', ' + row + ']';
    if (client.turn) {
      buf += ' - Your turn!';
    }
    buf += '\n\r'
    buf += '\n\r'
    buf += '\n\r'
    for(var j=0; j<client.height; j++) {
      for(var i=0; i<client.width; i++) {
        var v = client.board[j * client.width + i];
        buf += ' ';
        if (v > 0)
          buf += (v % 10);
        else
          buf += '-';
        buf += ' ';
      }
      buf += '\n\r'
      buf += '\n\r'
    }

    buf += '\x1b['+(row * 2 + 3)+';'+(col * 3 + 1)+'f***';
    buf += '\x1b['+(row * 2 + 4)+';'+(col * 3 + 1)+'f*';
    buf += '\x1b['+(row * 2 + 4)+';'+(col * 3 + 3)+'f*';
    buf += '\x1b['+(row * 2 + 5)+';'+(col * 3 + 1)+'f***';
  
    conn.write(buf);
  }

  function place() {
    console.log('place at col ' + col + ', row ' + row + '\n\r');
    client.place(col, row);
    redraw();
  }

  // game events

  client.on('connected', event => {
    console.log('Connected to game.');
  });

  client.on('event', event => {
    console.log('Got game event', event);
    redraw();
  });

  client.on('grid', event => {
    console.log('Game board updated', event);
    redraw();
  });

  client.on('turn', event => {
    console.log('Game turn updated', event);
    redraw();
  });

  // connect to game backend...

  conn.on('started', () => {
    console.log('Connection started');
    client.connect();
    redraw(conn);
  });

  conn.on('stopping', () => {
    console.log('Stopping connection');
    client.close();
  });

  conn.on('resize', () => {
    console.log('Connection resize');
  });

  conn.on('data', data => {
    console.log('Got data in connection', data);

    if (data.length === 1) {
      if (data[0] === '\n'.charCodeAt(0) || data[0] === '\r'.charCodeAt(0) || data[0] === ' '.charCodeAt(0)) {
        // enter
        place();
      }

      if (data[0] === 'q'.charCodeAt(0) || data[0] === 27) {
        conn.close();
        client.close();
      }
    }

    if (data.length === 3) {
      // check keystroke
      if (data[0] === 0x1b && data[1] === 0x5b) {
        if (data[2] === 0x44) {
          // left
          col -= 1;
          col += 10;
          col %= 10;
          redraw(conn);
        }
        if (data[2] === 0x43) {
          // right
          col += 1;
          col %= 10;
          redraw(conn);
        }
        if (data[2] === 0x41) {
          // up
          row -= 1;
          row += 10;
          row %= 10;
          redraw(conn);
        }
        if (data[2] === 0x42) {
          // down
          row += 1;
          row %= 10;
          redraw(conn);
        }
      }
    }
  });

  conn.start();
}

var server = new TelnetServer({handler});
server.start(PORT);
