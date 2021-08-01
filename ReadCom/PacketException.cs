using System;

namespace ReadCom
{
    public class PacketException : Exception
    {
        public PacketException(string message) : base(message)
        {
            
        }
    }
}