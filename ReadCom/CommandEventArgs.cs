using System;

namespace ReadCom
{
    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(Command cmd)
        {
            Command = cmd;
        }
        public Command Command { get; }
    }
}