using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using dotnetbackend.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace dotnet_backend
{

    public static class GameMiddlewareExtensions
    {
       

        public static IApplicationBuilder UseWebSockets(this IApplicationBuilder app, GameLogic game)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<GameMiddleware>(game);
        }
    }

    public class GameMiddleware
    {

        private readonly RequestDelegate _next;
        private GameLogic _game { get; set; }
        private List<WebSocket> activeSockets = new List<WebSocket>();
        private List<GameUserHandler> activeGames = new List<GameUserHandler>();

        public GameMiddleware(RequestDelegate next, GameLogic game)
        {
            _next = next;
            _game = game;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) {
                await _next.Invoke(context);
                return;
            }
            
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            activeSockets.Add(socket);
            var activeUser = new GameUserHandler(socket, _game);
            activeGames.Add(activeUser);
            while (!socket.CloseStatus.HasValue)
            {
                await activeUser.HandleRequest(context, socket);

            }

            await _next.Invoke(context);
        }

    }
}