using System.Net.Sockets;

SendMagicPacket("MAC:ADDRESS:GOES:HERE", "IP.ADDRESS.GOES.HERE", 7);
    
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
