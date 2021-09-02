namespace ReadCom
{
    public class Command
    {
        public const int COMMAND_TYPE_CONNECT = 1;
        public const int COMMAND_TYPE_DISCONNECT = 2;
        public const int COMMAND_TYPE_START_READING = 3;
        public const int COMMAND_TYPE_STOP_READING = 4;
        public const int COMMAND_TYPE_SET_TIME = 5;
        public const int COMMAND_TYPE_UPDATE = 6;
        public int commandType { get; set; }
        public CommandData data { get; set; }

    }
}