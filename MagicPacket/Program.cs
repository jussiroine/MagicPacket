using System.Diagnostics;
using System.Net.Sockets;
using System.Globalization;

string domain = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("MAGICPACKET_DNS") ?? "wol.example.com";

(string macAddress, string ipAddress) = ResolveParameters(domain);

SendMagicPacket(macAddress, ipAddress, 7);

static (string mac, string ip) ResolveParameters(string domain)
{
    var psi = new ProcessStartInfo("nslookup", $"-type=TXT {domain}")
    {
        RedirectStandardOutput = true,
        UseShellExecute = false
    };
    using var process = Process.Start(psi)!;
    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    string mac = string.Empty;
    string ip = string.Empty;

    foreach (var line in output.Split('\n'))
    {
        var trimmed = line.Trim();
        if (trimmed.Contains("text ="))
        {
            var text = trimmed.Substring(trimmed.IndexOf("text =") + 6).Trim().Trim('"');
            foreach (var token in text.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (token.StartsWith("mac=", StringComparison.OrdinalIgnoreCase))
                    mac = token.Substring(4);
                else if (token.StartsWith("ip=", StringComparison.OrdinalIgnoreCase))
                    ip = token.Substring(3);
            }
        }
    }

    return (mac, ip);
}

static void SendMagicPacket(string macAddress, string ipAddress, int port)
{
    var macBytes = macAddress.Split(':').Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray();

    var packet = new byte[6 + 16 * macBytes.Length];

    for (int i = 0; i < 6; i++)
        packet[i] = 0xFF;

    for (int i = 6; i < packet.Length; i += 6)
        Buffer.BlockCopy(macBytes, 0, packet, i, 6);

    using var client = new UdpClient(ipAddress, port)
    {
        EnableBroadcast = true
    };

    client.Send(packet, packet.Length);
}
