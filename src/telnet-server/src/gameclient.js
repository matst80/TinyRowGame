const EventEmitter = require('events');
const WebSocket = require('ws');

class GameClient extends EventEmitter {
  constructor(url) {
    super();
    this.url = url;
    this.socket = null;
    this.board = [];
    this.width = 10;
    this.height = 10;
    this.usernr = 0;
    this.userid = '';
    this.userlist = [];
    this.turn = 0;
    for(var i=0; i<this.width * this.height; i++) {
      this.board.push(0);
    }
  }

  _handleEvent(event) {
    console.log('GameClient: Handle event', event);

    if (event.type === 'init') {
      this.userid = event.user.id;
      this.usernr = event.user.nr;
      this.board = [];
      for(var i=0; i<this.width * this.height; i++) {
        this.board.push(0);
      }
      event.points.forEach(e => {
        this.board[e.y * this.width + e.x] = e.v;
      })
      this.emit('loggedin', event);
    }

    if (event.type === 'userlist') {
      this.userlist = event.users;
      this.emit('userlist', event);
    }

    if (event.type === 'turn') {
      this.turn = 1;
      this.emit('turn', event);
    }

    if (event.type === 'grid') {
      // this.board[]
      this.board = [];
      for(var i=0; i<this.width * this.height; i++) {
        this.board.push(0);
      }
      event.grid.forEach(e => {
        this.board[e.y * this.width + e.x] = e.v;
      })
      this.emit('grid', event);
    }

    this.emit('event', event);
  }

  place(x, y) {
    this.socket.send(JSON.stringify({type: 'place', x, y}));
    this.turn = 0;
  }

  connect() {
    this.socket = new WebSocket(this.url);

    this.socket.onmessage = msg => {
      const event = JSON.parse(msg.data);
      this._handleEvent(event);
    }

    this.socket.onopen = () => {
      console.log('GameClient: Opened');
      this.emit('connected');
    }
  }

  close() {
    console.log('GameClient: Closing connection');
    this.socket.close();
    this.socket = null;
  }
}

module.exports = GameClient;
