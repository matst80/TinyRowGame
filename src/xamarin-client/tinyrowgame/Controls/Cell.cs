using System;
using System.Linq;
using tinyrowgame.Extensions;
using tinyrowgame.Logic;
using tinyrowgame.SocketCommands;
using Xamarin.Forms;

namespace tinyrowgame.Controls
{
    public class Cell : Layout<View>
    {
        readonly CellPosition _cellpos;
        Label _label = new Label()
        {
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Color.White
        };

        private readonly Color[] DEFAULT_COLORS = { Color.White, Color.Red, Color.Blue, Color.Green, Color.Aqua };

        public Cell(CellPosition cellpos)
        {
            this._cellpos = cellpos;
            _label.Text = cellpos.Value.ToString();
            BackgroundColor = DEFAULT_COLORS[cellpos.Value];
            _label.Opacity = 0;
        }

        public void PlaceInGrid(double v)
        {
            this.LayoutTo(new Rectangle(CellData.RealX*v,CellData.RealY*v,v,v));
        }

        public CellPosition CellData => _cellpos;

        protected override void OnParentSet()
        {
            base.OnParentSet();
            Children.Add(_label);
            _label.FadeTo(1);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            _label.Layout(new Rectangle(x, y, width, height));
        }

        internal bool Match(Pos p)
        {
            return (p.Id == CellData.Id || (p.X == CellData.RealX && p.GridY == CellData.RealY));
        }

        internal void Update(Pos p)
        {
            if (p.Value!=CellData.Value) {
                this.ColorTo(BackgroundColor, DEFAULT_COLORS[p.Value],(_) => {
                    _label.Text = p.Value.ToString();
                });
            }
            CellData.GridPosition = p;
        }

        internal bool Match(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
