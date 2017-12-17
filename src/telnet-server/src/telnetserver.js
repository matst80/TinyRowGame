var net = require('net');
var EventEmitter = require('events');

class TelnetConnection extends EventEmitter {
  constructor (socket) {
    super();
    this.socket = socket;
    this.active = false;
    this.width = 80;
    this.height = 25;
    this._init();
  }

  write(data) {
    if (this.socket) {
      this.socket.write(data);
    }
  }

  start() {
    this.active = true;
    console.log('TelnetConnection: Sending telnet handshake');
    this.socket.write(new Buffer([
      0xff, 0xfd, 0x18,
      0xff, 0xfd, 0x00,
      0xff, 0xfd, 0x1f,
      0xff, 0xfd, 0x03,
      0xff, 0xfb, 0x03,
      0xff, 0xfb, 0x01,
      0xff, 0xfa, 0x18, 0x01, 0xff, 0xf0
    ]), {}, () => {
      this.socket.write('\x1b[?1005h');
      this.socket.write('\x1b[?1003h');
      this.socket.write('\x1b[?1000h'); // enable mouse
      this.socket.write('\x1b[2J'); // clear screen
      this.emit('started');
    });
  }

  _handleIACCommand(data) {
    console.log('TelnetConnection: Got IAC Command', data);

    // Init:
    // <Buffer ff fb 18 ff fb 00 ff fb 1f ff fa 1f 00 6a 00 1b ff f0 ff fb 03 ff fd 03 ff fd 01 ff fa 18 00 58 54 45 52 4d 2d 32 35 36 43 4f 4c 4f 52 ff f0>

    // Viewport resize:
    //            SB ws WW WW HH HH 
    // <Buffer ff fa 1f 00 6a 00 1b ff f0>

    if(data[1] === 0xFA && data[2] === 0x1f) {
      this.width = data[3] * 256 + data[4];
      this.height = data[5] * 256 + data[6];
      console.log('  Viewport size: ' + this.width + 'x' + this.height);
      this.emit('resize');
    }
  }

  _handleIAC(data) {
    console.log('TelnetConnection: Got IAC Commands', data);
    // bruteforce af.
    for(var i=0; i<data.length; i++) {
      if (data[i] === 255) {
        this._handleIACCommand(data.slice(i));
      }
    }
  }

  close() {
    console.log('TelnetConnection: Closing connection');
    this.active = false;
    this.socket.write('\x1b[?1000l'); // disable mouse
    this.socket.write('\x1b[?25h'); // show cursor
    this.socket.end('\r\n');
    // this.socket = null;
  }

  _init() {
    console.log('TelnetConnection: New socket connected');

    this.socket.on('data', data => {
      console.log('TelnetConnection: Receive data', data);

      if (data[0] === 255) {
        this._handleIAC(data);
        // parse events...
      } else {
        this.emit('data', data);
      }
    })

    this.socket.on('end', () => {
      console.log('TelnetConnection: Socket disconnected.');
      this.socket = null;
    })
  }
}

class TelnetServer {
  constructor({handler}) {
    this.handler = handler;
  }

  start(port) {
    console.log('TelnetServer: Listening on port ' + port);
    net.createServer(socket => this.handler(new TelnetConnection(socket))).listen(port);
  }
}

module.exports = TelnetServer;
