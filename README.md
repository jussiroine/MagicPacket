# MagicPacket

A lightweight .NET 8 CLI utility for sending Wake-on-LAN (WoL) magic packets to wake up network devices. What makes MagicPacket unique is its innovative approach of using DNS TXT records to store and retrieve target device configurations, making it perfect for automated deployments and containerized environments.

## Features

- 🚀 **DNS-based Configuration**: Store MAC addresses and IP addresses in DNS TXT records
- 🐳 **Docker Ready**: Designed to run seamlessly in Docker containers
- ⚡ **Lightweight**: Minimal footprint with .NET 8 runtime
- 🔧 **Flexible**: Support for command-line arguments and environment variables
- 🌐 **Network Agnostic**: Works across different network segments when properly configured

## How It Works

MagicPacket uses a unique approach to Wake-on-LAN by leveraging DNS TXT records to store device configuration. Instead of hardcoding MAC addresses and IP addresses, the application:

1. **DNS Query**: Performs an `nslookup` query for TXT records on a specified domain
2. **Configuration Parsing**: Extracts `mac=` and `ip=` parameters from the TXT record
3. **Magic Packet Construction**: Creates a standard WoL magic packet (6 bytes of 0xFF + 16 repetitions of MAC address)
4. **Network Transmission**: Sends the packet via UDP to the target IP address on port 7

This approach enables:
- **Dynamic Configuration**: Update device settings without rebuilding containers
- **Centralized Management**: Manage multiple devices through DNS
- **Version Control**: Track configuration changes through DNS management
- **Security**: No hardcoded credentials in application code

## Requirements

- .NET 8.0 Runtime (for direct execution)
- Docker (for containerized deployment)
- Network access to DNS server and target devices
- Target devices must support Wake-on-LAN and be properly configured

## Installation

### Docker (Recommended)

Pull the pre-built image:
```bash
docker pull jussiroine/magicpacket:latest
```

Or build from source:
```bash
git clone https://github.com/jussiroine/MagicPacket.git
cd MagicPacket
docker build -t magicpacket .
```

### Direct .NET Execution

```bash
git clone https://github.com/jussiroine/MagicPacket.git
cd MagicPacket/MagicPacket
dotnet run [domain]
```

## Usage

### Docker Container

```bash
# Using command line argument
docker run --rm magicpacket wol.example.com

# Using environment variable
docker run --rm -e MAGICPACKET_DNS=wol.example.com magicpacket

# One-liner for quick wake-up
docker run --rm magicpacket my-pc.home.local
```

### Direct Execution

```bash
# Using command line argument
dotnet run wol.example.com

# Using environment variable
MAGICPACKET_DNS=wol.example.com dotnet run
```

## Configuration

### DNS TXT Record Setup

Configure your DNS server to include TXT records with device information:

```dns
wol.example.com.    IN    TXT    "mac=00:11:22:33:44:55 ip=192.168.1.100"
pc1.wol.example.com. IN    TXT    "mac=AA:BB:CC:DD:EE:FF ip=192.168.1.101"
server.wol.example.com. IN TXT    "mac=12:34:56:78:9A:BC ip=10.0.0.50"
```

### Parameter Format

- **mac=**: MAC address in colon-separated hexadecimal format (e.g., `00:11:22:33:44:55`)
- **ip=**: Target IP address or broadcast address (e.g., `192.168.1.255`)

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `MAGICPACKET_DNS` | DNS domain to query for TXT records | `wol.example.com` |

## Examples

### Basic Usage

```bash
# Wake up a device configured at wol.mynetwork.com
docker run --rm magicpacket wol.mynetwork.com
```

### Multiple Devices

```bash
# Wake up different devices
docker run --rm magicpacket desktop.wol.home.local
docker run --rm magicpacket server.wol.home.local
docker run --rm magicpacket nas.wol.home.local
```

### Scheduled Wake-up with Cron

```bash
# Add to crontab for daily 8 AM wake-up
0 8 * * * docker run --rm magicpacket workstation.wol.office.com
```

### Docker Compose

```yaml
version: '3.8'
services:
  wol-desktop:
    image: magicpacket
    command: ["desktop.wol.home.local"]
    restart: "no"
  
  wol-server:
    image: magicpacket
    environment:
      - MAGICPACKET_DNS=server.wol.home.local
    restart: "no"
```

## DNS TXT Record Examples

### Bind9 Zone File
```
$ORIGIN wol.example.com.
@               IN      TXT     "mac=00:11:22:33:44:55 ip=192.168.1.100"
desktop         IN      TXT     "mac=AA:BB:CC:DD:EE:FF ip=192.168.1.255"
server          IN      TXT     "mac=12:34:56:78:9A:BC ip=10.0.0.255"
```

### PowerDNS
```sql
INSERT INTO records (domain_id, name, type, content, ttl) VALUES 
(1, 'wol.example.com', 'TXT', 'mac=00:11:22:33:44:55 ip=192.168.1.100', 300);
```

### Cloudflare DNS
```bash
curl -X POST "https://api.cloudflare.com/client/v4/zones/{zone_id}/dns_records" \
  -H "Authorization: Bearer {api_token}" \
  -H "Content-Type: application/json" \
  --data '{"type":"TXT","name":"wol.example.com","content":"mac=00:11:22:33:44:55 ip=192.168.1.100"}'
```

## Building from Source

### Prerequisites
- .NET 8.0 SDK
- Docker (optional)

### Build Steps

```bash
# Clone repository
git clone https://github.com/jussiroine/MagicPacket.git
cd MagicPacket

# Build with .NET CLI
dotnet build

# Run locally
cd MagicPacket
dotnet run wol.example.com

# Build Docker image
docker build -t magicpacket .

# Run in Docker
docker run --rm magicpacket wol.example.com
```

## Troubleshooting

### Common Issues

1. **DNS Resolution Fails**
   - Verify DNS server is accessible
   - Check TXT record format and content
   - Test with `nslookup -type=TXT your-domain.com`

2. **Device Doesn't Wake Up**
   - Ensure Wake-on-LAN is enabled in BIOS/UEFI
   - Verify network card supports WoL
   - Check MAC address format in DNS record
   - Confirm target device is in sleep/hibernation (not powered off)

3. **Network Connectivity**
   - Verify UDP port 7 is not blocked
   - Check if broadcast packets are allowed
   - Ensure container has network access

### Debug Commands

```bash
# Test DNS resolution
nslookup -type=TXT wol.example.com

# Verify Docker network
docker run --rm --network host magicpacket wol.example.com

# Check container logs
docker run --rm magicpacket wol.example.com
```

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## License

This project is available under the MIT License. See the LICENSE file for more details.

## Acknowledgments

- Built with .NET 8
- Containerized with Docker
- Inspired by the need for flexible, DNS-driven network device management
