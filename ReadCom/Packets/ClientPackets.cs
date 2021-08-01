namespace ReadCom.Packets
{
    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        unknown = 0,
        startReading = 1,
        stopReading = 2,
        rewind = 3,
        stopRewind = 4,
        setTime = 5,
        getTime = 6,
        getStatus = 7,
        startData = 8,
        stopData = 9,
    }
}