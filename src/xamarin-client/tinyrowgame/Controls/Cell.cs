using System;
using System.Linq;
using SkiaSharp.Views.Forms;
using TinyRowMessages.SocketCommands;
using tinyrowgame.Extensions;
using tinyrowgame.Logic;
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
            TextColor = Color.Black
        };

        private readonly Color[] DEFAULT_COLORS = { Color.Transparent, Color.Red, Color.Blue, Color.Green, Color.Aqua };

        public Cell(CellPosition cellpos)
        {
            this._cellpos = cellpos;
            _label.Text = cellpos.Value.ToString();
            BackgroundColor = DEFAULT_COLORS[cellpos.Value];
            _label.Opacity = 0;

        }

        public void PlaceInGrid(double v)
        {
            this.Layout(new Rectangle(CellData.RealX * v, CellData.RealY * v, v, v));
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
            return (p.Id == CellData.Id || Match(p.X, p.Y));
        }

        internal void Update(Pos p)
        {
            if (p.Value != CellData.Value)
            {
                BackgroundColor = DEFAULT_COLORS[p.Value % DEFAULT_COLORS.Length];
                _label.Text = p.Value.ToString();
                //this.ColorTo(BackgroundColor, DEFAULT_COLORS[p.Value % DEFAULT_COLORS.Length],(_) => {

                //},350, Easing.CubicInOut);
            }
            CellData.GridPosition = p;
        }

        internal bool Match(int x, int y)
        {
            return (x == CellData.RealX && y == CellData.RealY);
        }

        internal void Reset()
        {

            BackgroundColor = DEFAULT_COLORS[0];
            _label.Text = "0";
            //this.ColorTo(BackgroundColor, DEFAULT_COLORS[p.Value % DEFAULT_COLORS.Length],(_) => {

            //},350, Easing.CubicInOut);

        }
    }
}
