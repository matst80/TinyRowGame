using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using dotnetbackend;
using dotnetbackend.ClientApp.Extensions;
using dotnetbackend.Contracts;
using dotnetbackend.Logic;
using Microsoft.AspNetCore.Http;
using TinyRowMessages.SocketCommands;
using TinyWebSockets;
using TinyWebSockets.Interfaces;

namespace dotnet_backend
{
    internal class GameUserHandler : IMessageReceiver
    {
              

        private WebSocket webSocket;
        private GameLogic game;
        private int UserNumber;
        private MessageHandler sockeMessageHandler;
        public bool Connected = false;

        public GameUserHandler(WebSocket webSocket, GameLogic game)
        {
            this.webSocket = webSocket;
            this.game = game;
            Connected = true;

            var socketGame = new GameSocketServer(webSocket);
            this.sockeMessageHandler = new MessageHandler(socketGame);
            var initMessage = new Init();
            sockeMessageHandler.PopulateActions(initMessage);
            sockeMessageHandler.RegisterActionReceiver(this);

            socketGame.OnDisconnect += SocketGame_OnDisconnect;
            socketGame.OnError += (sender, e) => {
                var i = 3;
            };

            game.OnWinner += Game_OnWinner;
           
            game.OnTurnChange += Game_OnTurnChange;

            game.OnUsersChange += Game_OnUsersChange; 
               
            game.OnGridChange += Game_OnGridChange;

            UserNumber = game.AddUser();
            initMessage.UserData = new User()
            {
                Id = "",
                Nr = UserNumber
            };
            initMessage.Points = game.GetGameGrid().ToGrid().Grid;
            sockeMessageHandler.SendMessage(initMessage);
        }

        public bool IsActive => true;

        void SocketGame_OnDisconnect(object sender, System.EventArgs e)
        {
            //TODO Remove eventhandlers
            game.OnWinner -= Game_OnWinner;

            game.OnTurnChange -= Game_OnTurnChange;

            game.OnUsersChange -= Game_OnUsersChange;

            game.OnGridChange -= Game_OnGridChange;
            Connected = false;
        }

        void Game_OnGridChange(object sender, GameGrid e)
        {
            sockeMessageHandler.SendMessage(e.ToGrid());
        }

        void Game_OnUsersChange(object sender, System.Collections.Generic.IList<int> e)
        {
            sockeMessageHandler.SendMessage(new UserList()
            {
                Users = e
            });
        }

        internal async Task HandleRequest(HttpContext context, WebSocket webSocket)
        {
            await Task.Run(() =>
            {
                while (Connected)
                {
                    Task.Delay(100);
                }
            });
        }

        void Game_OnTurnChange(object sender, int e)
        {
            if (UserNumber == e)
                sockeMessageHandler.SendMessage(new TurnToken());
            else
                sockeMessageHandler.SendMessage(new OtherUserTurn()
                {
                    UserNr = e
                });
        }   

        void Game_OnWinner(object sender, int e)
        {
            sockeMessageHandler.SendMessage(new Winner()
            {
                WinnerUserNr = e
            });
        }  

        public void HandleAction(IMessage action)
        {
            switch (action)
            {
                case PlaceMarker place:
                    game.Place(place.ToGame(UserNumber));
                    break;
            }
        }

        public void SetStateService(MessageHandler stateService)
        {

        }
    }
}