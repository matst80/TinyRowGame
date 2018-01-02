using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tinyrowgame.SocketCommands;
using TinyWebSockets;
using TinyWebSockets.Interfaces;
using Xamarin.Forms;

namespace tinyrowgame.Controls
{
    public class GridControl : Layout<Cell>, IMessageReceiver
    {


        private MessageHandler _messageHandler;

        public bool IsActive => true;
        public int UserNr { get; internal set; }


        protected override void OnParentSet()
        {
            base.OnParentSet();
            Service.MessageHandler.PopulateActions(new Init());
            Service.MessageHandler.RegisterActionReceiver(this);

            Task.Run(async () => { 
                await Service.SocketService.StartListening(new Uri("ws://fw.knatofs.se:8001"));
            });
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            foreach (var child in Children)
            {
                child.PlaceInGrid(Math.Min(width, height) / Math.Max(RowCount, CellCount));
                //child.CellData.RealPosition
            }
        }

        public void SetStateService(MessageHandler stateService)
        {
            _messageHandler = stateService;
        }

        public void HandleAction(IMessage action)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    switch (action)
                    {
                        case GridPositions grid:
                            UpdateGrid(grid.Grid);
                            break;
                        case Init init:
                            UpdateGrid(init.Points);
                            break;
                    }
                }
                catch(Exception ex) {
                    throw ex;    
                }
            });
        }

        private const int maxVal = int.MaxValue;
        private const int minGridSize = 10;

        //private Dictionary<int, Cell> PlacedPositions = new Dictionary<int, Cell>();

        public int RowCount { get; set; } = minGridSize;
        public int CellCount { get; set; } = minGridSize;

        public Cell CreateCell(int x, int y)
        {
            var ret = new Cell(new Logic.CellPosition(x, y));
            var setValue = new TapGestureRecognizer();
            setValue.Tapped += (object sender, EventArgs e) =>
            {
                if (ret.CellData.IsFree())
                {
                    Service.MessageHandler.SendMessage(new PlaceMarker()
                    {
                        X = ret.CellData.RealX - offsetX,
                        Y = ret.CellData.RealY - offsetY
                    });
                }
            };
            ret.GestureRecognizers.Add(setValue);
            return ret;
        }

        private void UpdateGrid(IList<Pos> grid)
        {
            NormalizeGrid(grid);

            foreach (Pos p in grid)
            {
                var existing = Children.FirstOrDefault(d => d.Match(p));
                if (existing == null)
                {
                    existing = CreateCell(p.GridX, p.GridY);
                    try
                    {
                        Children.Add(existing);
                    }
                    catch(Exception ex) {
                        throw ex;
                    }
                }
                existing.Update(p);

            }

            for (var y = 0; y < RowCount; y++)
            {
                for (var x = 0; x < CellCount; x++)
                {
                    var existing = Children.FirstOrDefault(d => d.Match(x, y));
                    if (existing == null)
                    {
                        existing = CreateCell(x, y);
                        //existing.PlaceInGrid(20);
                        Children.Add(existing);
                    }
                }
            }
            //ForceLayout();
        }

        private int offsetX;
        private int offsetY;

        private void NormalizeGrid(IList<Pos> grid)
        {
            var minX = maxVal;
            var minY = maxVal;
            var maxX = -maxVal;
            var maxY = -maxVal;

            foreach (Pos p in grid)
            {
                minX = Math.Min(p.X, minX);
                minY = Math.Min(p.Y, minY);
                maxX = Math.Max(p.X, minX);
                maxY = Math.Max(p.Y, maxY);
            }

            CellCount = Math.Max((maxX - minX) + 2, minGridSize);
            RowCount = Math.Max((maxY - minY) + 2, minGridSize);

            foreach (Pos p in grid)
            {
                p.Normalize(minX, minY);
            }

            //offsetX = minX;
            //offsetY = minY;
        }
    }
}
