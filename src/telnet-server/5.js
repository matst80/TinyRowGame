const TelnetServer = require('./src/telnetserver.js');
const GameClient = require('./src/gameclient.js');
const TextWrangler = require('./src/textwrangler.js');

const PORT = process.env.PORT || 23;
const GAMESERVER = process.env.SERVER || 'ws://localhost:8001';

const playerbgcolors = [
  TextWrangler.Colors.RED,
  TextWrangler.Colors.MAGENTA,
  TextWrangler.Colors.BLUE,
  TextWrangler.Colors.GREEN,
  TextWrangler.Colors.YELLOW,
];

const playerfgcolors = [
  TextWrangler.Colors.LIGHT + TextWrangler.Colors.GRAY,
  TextWrangler.Colors.LIGHT + TextWrangler.Colors.GRAY,
  TextWrangler.Colors.LIGHT + TextWrangler.Colors.GRAY,
  TextWrangler.Colors.BLACK,
  TextWrangler.Colors.BLACK,
];

function handler(conn) {
  console.log('Got new connection');

  var client = new GameClient(GAMESERVER);
  var gfx = new TextWrangler();

  var col = 0;
  var row = 0;
  var bx, by;
  var player = 0;
  var particles = [];
  var active = true;

  function spawnparticle(opts) {
    particles.push({
      ttl: opts.ttl || 5.0,
      x: opts.x || 5.0,
      y: opts.y || 5.0,
      vx: opts.vx || 5.0,
      vy: opts.vy || 5.0,
      ch: opts.ch || 5.0,
      fg: opts.fg || 5.0,
    });
  }

  function redraw() {
    // console.log('redraw, board', JSON.stringify(client.board));

    gfx.clear();
    gfx.goto(0, 0);

    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.GRAY);
    gfx.write('5-I-RAD');

    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
    gfx.write(' - ');

    gfx.setBackground(playerbgcolors[client.usernr % playerbgcolors.length]);
    gfx.setColor(playerfgcolors[client.usernr % playerfgcolors.length]);
    gfx.write(' Player ' + client.usernr.toString() + ' ');

    gfx.setBackground(TextWrangler.Colors.BLACK);
    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
    // gfx.write(' of ' + JSON.stringify(client.userlist));

    if (client.turn) {
      gfx.write(' - ');
      gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.YELLOW);
      gfx.write('Your turn!');
    }

    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
    for(var j=0; j<client.height; j++) {
      for(var i=0; i<client.width; i++) {
        var v = client.board[j * client.width + i];
        gfx.goto(bx + 4 * i, by + 0 + 2 * j); gfx.write('┼───┼');
        gfx.goto(bx + 4 * i, by + 1 + 2 * j); gfx.write('│ ? │');
        gfx.goto(bx + 4 * i, by + 2 + 2 * j); gfx.write('┼───┼');
      }
    }
    for(var j=1; j<client.width; j++) {
      gfx.goto(bx + 4 * j, by); gfx.write('┬');
      gfx.goto(bx + 4 * j, by + 2 * client.height); gfx.write('┴');
    }
    for(var j=1; j<client.height; j++) {
      gfx.goto(bx, by + 2 * j); gfx.write('├');
      gfx.goto(bx + 4 * client.width, by + 2 * j); gfx.write('┤');
    }

    gfx.goto(bx, by); gfx.write('╭');
    gfx.goto(bx + 4 * client.width, by); gfx.write('╮');
    gfx.goto(bx, by + 2 * client.height); gfx.write('╰');
    gfx.goto(bx + 4 * client.width, by + 2 * client.height); gfx.write('╯');

    if (client.turn) {
      gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.YELLOW);
    } else {
      gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.CYAN);
    }
    gfx.goto(bx - 1 + 4 * col, by + 0 + 2 * row);gfx.write(' ╔═══╗ ');
    gfx.goto(bx - 1 + 4 * col, by + 1 + 2 * row);gfx.write(' ║   ║ ');
    gfx.goto(bx - 1 + 4 * col, by + 2 + 2 * row);gfx.write(' ╚═══╝ ');

    for(var j=0; j<client.height; j++) {
      for(var i=0; i<client.width; i++) {
        var v = client.board[j * client.width + i];
        if (v > 0) {
          gfx.setBackground(playerbgcolors[v % playerbgcolors.length]);
          gfx.setColor(playerfgcolors[v % playerfgcolors.length]);
          gfx.goto(bx + 1 + 4 * i, by + 1 + 2 * j);
          gfx.write(' ' + (v % 10).toString() + ' ');
        } else {
          gfx.setBackground(TextWrangler.Colors.BLACK);
          gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
          gfx.goto(bx + 1 + 4 * i, by + 1 + 2 * j);
          gfx.write(' - ');
        }
      }
    }

    gfx.goto(0, gfx.height-1);

    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
    gfx.write('Use arrow keys to navigate, Use ');
    gfx.setColor(TextWrangler.Colors.GRAY); gfx.write('[');
    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.YELLOW); gfx.write('Space');
    gfx.setColor(TextWrangler.Colors.GRAY); gfx.write(']');
    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
    gfx.write(' or ');
    gfx.setColor(TextWrangler.Colors.GRAY); gfx.write('[');
    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.YELLOW); gfx.write('Enter');
    gfx.setColor(TextWrangler.Colors.GRAY); gfx.write(']');
    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
    gfx.write(' to select, Press ');
    gfx.setColor(TextWrangler.Colors.GRAY); gfx.write('[');
    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.YELLOW); gfx.write('Q');
    gfx.setColor(TextWrangler.Colors.GRAY); gfx.write(']');
    gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
    gfx.write(' to exit');

    particles.forEach(t => {
      gfx.goto(Math.round(t.x), Math.round(t.y));
      if (t.ttl < 2) {
        gfx.setColor(TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLACK);
      } else if (t.ttl < 4) {
        gfx.setColor(TextWrangler.Colors.GRAY);
      } else {
        gfx.setColor(t.fg);
      }
      gfx.write(t.ch);
    });

    buf = gfx.getVT();
    console.log('Sending ' + buf.length + ' byte screen update...');
    conn.write(buf);
  }

  function place() {
    console.log('place at col ' + col + ', row ' + row + '\n\r');
    client.place(col, row);
    spawntest1(3, bx + 3 + 4 * col, by + 1 + 2 * row);
    redraw();
  }

  // game events

  client.on('connected', event => {
    console.log('Connected to game.');
  });

  client.on('event', event => {
    console.log('Got game event', event);
    // if (event.type === 'maxlen') {
    //   // highlight longest series
    //   event.connected.forEach(t => {
    //     spawntest1(11, bx + 3 + 4 * t.x, by + 1 + 2 * t.y);
    //   });
    // }
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

  function resize(w, h) {
    gfx.resize(w, h);
    var tw = client.width * 4 + 1;
    var th = client.height * 2 + 1;
    bx = Math.floor((gfx.width - tw) / 2);
    by = Math.floor((gfx.height - th) / 2);
    redraw();
  }

  conn.on('started', () => {
    console.log('Connection started');
    client.connect();
    resize(conn.width, conn.height);
  });

  conn.on('stopping', () => {
    console.log('Stopping connection');
    client.close();
  });

  conn.on('resize', () => {
    console.log('Connection resize');
    resize(conn.width, conn.height);
  });

  function spawntest1(N, x, y, dx, dy) {
    for(var i=0; i<N; i++) {
      var chs = ['*', '.', '+', '#', '+'];
      var fgs = [
        TextWrangler.Colors.LIGHT + TextWrangler.Colors.CYAN,
        TextWrangler.Colors.LIGHT + TextWrangler.Colors.BLUE,
        TextWrangler.Colors.LIGHT + TextWrangler.Colors.GREEN,
        TextWrangler.Colors.LIGHT + TextWrangler.Colors.RED,
        TextWrangler.Colors.LIGHT + TextWrangler.Colors.YELLOW,
        TextWrangler.Colors.LIGHT + TextWrangler.Colors.GRAY,
      ];
      spawnparticle({
        x,
        y,
        vx: -3 + Math.random() * 6,
        vy: -2 + Math.random() * 4,
        ch: chs[Math.floor(Math.random() * chs.length)],
        fg: fgs[Math.floor(Math.random() * fgs.length)],
        ttl: 8 + Math.random() * 5.0,
      })
    }
  }

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
          redraw();
        }
        if (data[2] === 0x43) {
          // right
          col += 1;
          col %= 10;
          redraw();
        }
        if (data[2] === 0x41) {
          // up
          row -= 1;
          row += 10;
          row %= 10;
          redraw();
        }
        if (data[2] === 0x42) {
          // down
          row += 1;
          row %= 10;
          redraw();
        }
      }
    }
  });

  conn.start();

  function tick() {

    if (particles.length > 0) {
      particles.forEach(t => {
        t.vx *= 0.9;
        t.vy *= 0.9;
        t.vy += 0.15; // gravity
        t.x += t.vx;
        t.y += t.vy;
        t.ttl -= 0.1;
      });

      particles = particles.filter(t => t.ttl > 0);

      redraw();
    }

    if (conn.active) {
      queueTick();
    }
  }

  function queueTick() {
    setTimeout(tick, 100);
  }

  queueTick();
}

var server = new TelnetServer({handler});
server.start(PORT);
