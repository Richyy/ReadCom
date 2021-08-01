namespace ReadCom.Packets
{
    /// <summary>Sent from server to client.</summary>
    public enum ServerPacketTypes
    {
        unknown = 0,
        connected = 1,
        voltage = 2,
        chiptime = 3,
        getTime = 4,
    }
}