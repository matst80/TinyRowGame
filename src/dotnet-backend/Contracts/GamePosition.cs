using System;

namespace dotnetbackend.Contracts
{

    public class GamePosition
    {
        public GamePosition()
        {

        }

        public GamePosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; internal set; }

        public int Y { get; internal set; }

        public int Id { get; set; }

        public int Value { get; set; }

        internal bool Match(GamePosition pos)
        {
            return (X == pos.X && Y == pos.Y);
        }

        internal GamePosition Move(DirectionEnum dir)
        {
            var diffX = 0;
            var diffY = 0;
            switch (dir)
            {
                case DirectionEnum.Up: // up |
                    diffY = 1;
                    diffX = 0;
                    break;
                case DirectionEnum.UpRight: // up-right /
                    diffY = 1;
                    diffX = 1;
                    break;
                case DirectionEnum.Right: // right -
                    diffY = 0;
                    diffX = 1;
                    break;
                case DirectionEnum.DownRight: // down-right \
                    diffY = -1;
                    diffX = 1;
                    break;
                case DirectionEnum.Down: // down |
                    diffY = -1;
                    diffX = 0;
                    break;
                case DirectionEnum.DownLeft: // down-left /
                    diffY = -1;
                    diffX = -1;
                    break;
                case DirectionEnum.Left: // left -
                    diffY = 0;
                    diffX = -1;
                    break;
                case DirectionEnum.UpLeft: // up-left \
                    diffY = -1;
                    diffX = -1;
                    break;
            }
            return new GamePosition(Y + diffX, Y + diffY);
        }

        internal GamePosition MoveInverted(DirectionEnum dir) {
            return Move(dir + 4);
        }

    }
}
