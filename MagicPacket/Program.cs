using System.Net.Sockets;

SendMagicPacket("04:42:1A:95:B0:93", "192.168.0.2", 7);
    
void SendMagicPacket(string macAddress, string ipAddress, int port)
{
    var macBytes = macAddress.Split(':').Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();

    var packet = new byte[6 + 16 * macBytes.Length];

    for (int i = 0; i < 6; i++)
        packet[i] = 0xFF;

    for (int i = 6; i < packet.Length; i += 6)
        Buffer.BlockCopy(macBytes, 0, packet, i, 6);

    var client = new UdpClient(ipAddress, port);

    client.Send(packet, packet.Length);
}
