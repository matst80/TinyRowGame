using System;
using System.Linq;
using dotnetbackend.Contracts;
using TinyRowMessages.SocketCommands;

namespace dotnetbackend.ClientApp.Extensions
{
    public static class MessageExtensions
    {
        public static Pos ToPos(this GamePosition pos)
        {
            return new Pos()
            {
                X = pos.X,
                Y = pos.Y,
                Value = pos.Value,
                Id = pos.Value
            };
        }

        public static GamePosition ToGame(this Pos pos)
        {
            return new GamePosition(pos.X, pos.Y)
            {
                Value = pos.Value,
                Id = pos.Id
            };
        }

        public static GamePosition ToGame(this PlaceMarker marker, int userNumber)
        {
            return new GamePosition(marker.X, marker.Y)
            {
                Value = userNumber
            };
        }

        public static GridPositions ToGrid(this GameGrid grid)
        {
            return new GridPositions()
            {
                Grid = grid.Points.Select(d => d.ToPos()).ToList()
            };
        }
    }
}
