import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';

const gameEngine = function() {
  var t = this;
  t.handlers = {};
  
  function addHandler(key,cb) {
    if (t.handlers[key])
      t.handlers[key].push(cb);
    else
    t.handlers[key] = [cb];
  }

  function emit(key) {
    
    let argArray = Array.from(arguments);
    let calldata = argArray.slice(1);
    let hdls = t.handlers[key];

    if (hdls) {
      hdls.map(function(v,i){
        v.apply(this, calldata);
      });
    }
  }

  this.on = function(key,cb) {
    addHandler(key,cb);
  }

  this.place = function(pos) {
    pos.type='place';
    socket.send(JSON.stringify(pos));
  }

  var socket = new WebSocket("ws://192.168.1.131:8001");
  socket.onopen = function() {
      console.log('open');
      socket.onmessage = function(msg) {
          var jsonMsg = JSON.parse(msg.data);
          console.log(jsonMsg);   
          if (jsonMsg.type) {
            emit(jsonMsg.type,jsonMsg);
          }
                    
      }
  }  
}

class Cell extends Component {
  constructor(props) {
    super(props);
    this.state = props;
    
    this.makeTurn = this.makeTurn.bind(this);
  }
  classes() {
    return "cell clr"+this.props.value;
  }
  makeTurn() {
    console.log('clicketiclick');
    game.place({x:this.props.x,y:this.props.y});
  }
  render() {
    return <div className={this.classes()} onClick={this.makeTurn}>{this.props.value}</div>
  }
}

var game = new gameEngine();

const minSize = 10;

class Grid extends Component {
  constructor(props) {
    super(props);
    var t = this;
    this.grid = [];
    this.rows = minSize;
    this.cols = minSize;
    this.state = { grid: [[]]};
    
    game.on('grid',function (data){
      console.log('grid', data);
      t.updateGrid(data.grid);
    });
    game.on('init',function (griddata){
      console.log('init', griddata);
      t.updateGrid(griddata.points);
    });
  }
  getVal(x,y) {
    var ret = '';
    this.gridData.map(function(v) {
      if (v.x==x && v.y==y)
        ret = v.v;
    });
    return ret;
  }
  updateGrid(data) {
    
    
    this.setGridSize(data);
    console.log('griddata',this.rows,this.cols);
    var grid = [];
    for(var y=0;y<this.rows;y++) {
      var row = [];
      grid.push(row);
      for(var x=0;x<this.cols;x++) {
        var val = this.getVal(x,y,data);
        row.push(val);
      }  
    }
    console.log('newgrid',grid);
    this.setState({grid:grid});
  }
  setGridSize(data) {
    
    this.gridData = [];
    if (data && data.length)
    {    
      const maxVal = 999999999;
      let minX = maxVal;
      let minY = maxVal;
      let maxX = -maxVal;
      let maxY = -maxVal;

      data.map(function(v) {
        minX = Math.min(v.x,minX);
        minY = Math.min(v.y,minY);
        maxX = Math.max(v.x,maxX);
        maxY = Math.max(v.y,maxY);
      });
      
      var changeX = 0;
      var changeY = 0;

      if (minX < 0) {
        changeX = Math.abs(minX);
        maxX += changeX;
      }
      if (minY < 0) {
        changeY = Math.abs(minY);
        maxY += changeY;
      }
      
      this.rows = Math.max(minSize,(maxY-minY)+2);
      this.cols = Math.max(minSize,(maxX-minX)+2);

      this.gridData = data.map(function(v) {
        v.x+=(changeX);
        v.y+=(changeY);
        return v;
      });
    }
  }
  render() {
  
    return <div className="grid">{ this.state.grid.map((r,y) => {
      return <div className="row">{ r.map((c,x) => {
        return <Cell x={x} y={y} value={c} />
      })}</div> }) }</div>
  }
}

class App extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <h1 className="App-title">Fem i rad</h1>
        </header>
        <Grid />
      </div>
    );
  }
}

export default App;
