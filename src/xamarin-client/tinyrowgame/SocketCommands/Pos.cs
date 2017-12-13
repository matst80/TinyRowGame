using System;
using Newtonsoft.Json;

namespace tinyrowgame.SocketCommands
{
    public class Pos
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        public int DiffX { get; set; } = 0;
        public int DiffY { get; set; } = 0;

        public int GridX {
            get {
                return X - DiffX;
            }
        }

        public int GridY
        {
            get
            {
                return Y - DiffY;
            }
        }

        [JsonProperty("v")]
        public int Value { get; set; }

        public void Normalize(int normalizeX, int normalizeY)
        {
            //DiffX = normalizeX;
            //DiffY = normalizeY;
        }
    }
}
