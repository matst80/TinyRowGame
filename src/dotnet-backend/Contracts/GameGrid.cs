using System;
using System.Collections.Generic;
using System.Linq;
using TinyRowMessages.SocketCommands;

namespace dotnetbackend.Contracts
{
    public class GameGrid
    {
        public GameGrid()
        {
            Points = new List<GamePosition>();
        }

        public IList<GamePosition> Points { get; internal set; }

        public void RemoveWithValue(int value)
        {
            var toRemove = Points.Where(d => d.Value == value).ToList();
            foreach (var p in toRemove)
            {
                Points.Remove(p);
            }
        }

        public void Clear(IList<GamePosition> lst = null)
        {
            if (lst == null)
                Points = new List<GamePosition>();
            foreach (var p in lst)
            {
                Points.Remove(p);
            }
        }

        public GamePosition FindPoint(GamePosition pos)
        {
            return Points.FirstOrDefault(d => d.Match(pos));
        }

        public int FindPointValue(GamePosition pos)
        {
            var ret = 0;
            var point = FindPoint(pos);
            if (point != null)
                ret = point.Value;
            return ret;
        }

        public IList<GamePosition> AddPoint(GamePosition v)
        {
            var ret = new List<GamePosition>();
            if (FindPoint(v) == null)
            {
                Points.Add(v);
                MeasureLength(v, ret);
            }
            return ret;
        }

        private void MeasureLength(GamePosition v, List<GamePosition> ret)
        {
            var winValue = v.Value;
            for (int i = 0; i < 4; i++)
            {
                var dirPos = v;
                var dirNeg = v.MoveInverted((DirectionEnum)i);
                var dirArr = new List<GamePosition>();

                while (FindPointValue(dirPos) == winValue)
                {
                    dirArr.Add(dirPos);
                    dirPos = dirPos.Move((DirectionEnum)i);
                }

                while (FindPointValue(dirNeg) == winValue)
                {
                    dirArr.Add(dirNeg);
                    dirNeg = dirNeg.MoveInverted((DirectionEnum)i);
                }

                if (dirArr.Count() > ret.Count())
                {
                    ret.Clear();
                    ret.AddRange(dirArr);
                }
            }
        }
    }
}