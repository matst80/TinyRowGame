using System;

namespace TinyWebSockets
{
    public class MessageAttribute : Attribute
    {
        public string TypeName
        {
            get;
            set;
        }

        public MessageAttribute(string jsonTypeString)
        {
            TypeName = jsonTypeString;
        }
    }
}
