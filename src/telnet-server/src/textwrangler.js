const Colors = {

    DEFAULT: 39,

    BLACK: 0,
    RED: 1,
    GREEN: 2,
    YELLOW: 3,
    BLUE: 4,
    MAGENTA: 5,
    CYAN: 6,
    GRAY: 7,

    // FG: 30,
    LIGHT: 60
    // BG: 40,

    // DARKGRAY: 90,
    // LIGHTRED: 91,
    // LIGHTGREEN: 92,
    // LIGHTYELLOW: 93,
    // LIGHTBLUE: 94,
    // LIGHTMAGENTA: 95,
    // LIGHTCYAN: 96,

    // WHITE: 47,
};

class TextWrangler {

    constructor() {
        this.x = 0;
        this.y = 0;
        this.fg = Colors.LIGHT + Colors.GRAY;
        this.bg = Colors.BLACK;
        this.width = 80;
        this.height = 50;
        this.cells = [];
        this.lastcells = [];
    }

    resize(w, h) {
        this.width = w;
        this.height = h;
        this.cells = [];
        this.lastcells = [];
        for(var j=0; j<this.width*this.height; j++) {
            this.cells.push({
                ch: ' ',
                fg: Colors.LIGHT + Colors.GRAY,
                bg: Colors.BLACK,
            });
            this.lastcells.push({
                ch: '',
                fg: Colors.LIGHT + Colors.GRAY,
                bg: Colors.BLACK,
            });
        }
    }

    setColor(c) {
        this.fg = c;
    }

    setBackground(c) {
        this.bg = c;
    }

    goto(x, y) {
        this.x = x;
        this.y = y;
    }

    write(str) {
        for(var j=0; j<str.length; j++) {
            if (this.x >= 0 && this.y >= 0 && this.x < this.width && this.y < this.height) {
                var o = this.y * this.width + this.x;
                this.cells[o].ch = str.substring(j, j+1);
                this.cells[o].fg = this.fg;
                this.cells[o].bg = this.bg;
                this.x ++;
            }
        }
    }

    clear() {
        this.cells = [];
        for(var j=0; j<this.width*this.height; j++) {
            this.cells.push({
                ch: ' ',
                fg: Colors.LIGHT + Colors.GRAY,
                bg: Colors.BLACK,
            });
        }
    }

    getVT() {
        var buf = '';

        // buf += '\x1b[2J';
        // buf += '\x1b[2J\x1b[H\x1b[0m';
        buf += '\x1b[0m';
        buf += '\x1b[?25l';
        // buf += '\x1b[2J\x1b[0m';

        // detect changed chars and/or colors
        for(var j=0; j<this.height; j++) {
            var linechanged = false;
            var firstchange = 100;
            var lastchange = -100;

            for(var i=0; i<this.width; i++) {
                var o = j * this.width + i;
                if (this.cells[o].bg != this.lastcells[o].bg ||
                    this.cells[o].fg != this.lastcells[o].fg ||
                    this.cells[o].ch != this.lastcells[o].ch) {
                    if (i < firstchange) {
                        firstchange = i;
                        linechanged = true;
                    }
                    if (i > lastchange) {
                        lastchange = i;
                        linechanged = true;
                    }
                }
            }

            if (linechanged) {
                var lfg = -1;
                var lbg = -1;

                console.log('line #' + j + ' changed from column ' + firstchange + ' to ' + lastchange + '...');
                // queue goto x/y
                buf += '\x1b['+(j + 1)+';' + (firstchange + 1) + 'f';
                var o = j * this.width + firstchange;
                for(var i=firstchange; i<=lastchange; i++) {

                    if (this.cells[o].fg != lfg ||
                        this.cells[o].bg != lbg) {
                        // queue color change.

                        buf += '\x1b[' + (30 + this.cells[o].fg) + ';' + (40 + this.cells[o].bg) + 'm';

                        lfg = this.cells[o].fg;
                        lbg = this.cells[o].bg;
                    }

                    buf += this.cells[o].ch;
                    o ++;
                }
            }
        }

        // for(var j=0; j<this.width*this.height; j++) {
        //     var o = j;// this.y * this.width + this.x;
        //     if (this.cells[o].bg != this.lastcells[o].bg ||
        //         this.cells[o].fg != this.lastcells[o].fg) {
        //     }
        //     if (this.cells[o].ch != this.lastcells[o].ch) {
        //     }
        //     buf += this.cells[o].ch;
        // }

        for(var j=0; j<this.width*this.height; j++) {
            this.lastcells[j].ch = this.cells[j].ch;
            this.lastcells[j].bg = this.cells[j].bg;
            this.lastcells[j].fg = this.cells[j].fg;
        }

        return buf;
    }
}

TextWrangler.Colors = Colors;

module.exports = TextWrangler;
