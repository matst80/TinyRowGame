using System;
using tinyrowgame.SocketCommands;
using Xamarin.Forms;

namespace tinyrowgame.Logic
{
    public class CellPosition
    {
        public CellPosition(int x, int y)
        {
            RealX = x;
            RealY = y;
        }

        private Pos gridPosition;

        public Pos GridPosition
        {
            get
            {
                return gridPosition;
            }
            set
            {
                gridPosition = value;
            }
        }

        public int RealX { get; internal set; }
        public int RealY { get; internal set; }

        public int Value
        {
            get
            {
                if (gridPosition == null)
                    return 0;
                return gridPosition.Value;
            }
        }

        public int Id
        {
            get
            {
                if (GridPosition == null)
                    return -1;
                return GridPosition.Id;
            }
        }

        public bool IsFree()
        {
            return (gridPosition == null || gridPosition.Value == 0);
        }
    }
}
