using System;
using System.Collections.Generic;
using System.Linq;
using dotnetbackend.Contracts;
using TinyRowMessages.SocketCommands;

namespace dotnetbackend.Logic
{
    public class GameLogic
    {
        private int currentUser = -1;
        private int totalUsers = 0;

        private IList<int> users = new List<int>();
        private GameSettings settings = new GameSettings();
        private GameGrid grid = new GameGrid();
        private int pointInRowToWin = 5;

        public EventHandler<int> OnTurnChange;
        public EventHandler<IList<int>> OnUsersChange;
        public EventHandler<GameGrid> OnGridChange;
        public EventHandler<int> OnUserAdded;
        public EventHandler<int> OnUserRemoved;
        public EventHandler<IList<GamePosition>> OnTurnLength;
        public EventHandler<int> OnWinner;
        public EventHandler<int> OnReset;


        public int AddUser()
        {
            var addedUserNr = ++totalUsers;
            users.Add(addedUserNr);

            OnUserAdded?.Invoke(this, addedUserNr);
            OnUsersChange?.Invoke(this, users);

            if (currentUser == -1)
                SetNextUser();

            return addedUserNr;
        }

        private void SetNextUser()
        {
            if (currentUser == -1)
                currentUser = users.FirstOrDefault();
            else
            {
                var idx = users.IndexOf(currentUser);
                if (++idx > users.Count())
                {
                    idx = 0;
                }
                currentUser = users[idx];
            }
            OnTurnChange?.Invoke(this, currentUser);
        }

        public void RemoveUser(int userNr)
        {
            if (users.Contains(userNr))
            {
                users.Remove(userNr);
                grid.RemoveWithValue(userNr);

                if (!users.Any())
                {
                    Reset();
                }
                else
                {

                    OnUserRemoved?.Invoke(this, userNr);
                    OnUsersChange?.Invoke(this, users);
                    OnGridChange?.Invoke(this, grid);

                    if (currentUser == userNr)
                    {
                        SetNextUser();
                    }
                }
            }
        }

        public GameGrid GetGameGrid()
        {
            return grid;
        }

        private void Reset()
        {
            totalUsers = 0;
            currentUser = -1;
            users = new List<int>();
            OnReset?.Invoke(this, 0);
        }

        public void Place(GamePosition v)
        {
            if (v.Value == currentUser)
            {
                var ret = grid.AddPoint(v);
                if (ret.Any())
                {
                    if (ret.Count() >= pointInRowToWin)
                    {
                        OnWinner?.Invoke(this, v.Value);
                        grid.Clear(ret);
                    }
                    OnTurnLength?.Invoke(this, ret);
                    OnGridChange?.Invoke(this, grid);
                    SetNextUser();
                }
            }
        }
    }
}
