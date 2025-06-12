# MagicPacket

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A lightweight and efficient Wake-on-LAN (WoL) utility built with .NET 8, designed to remotely wake up network devices using Magic Packets. The application resolves target device information via DNS TXT records and can be easily deployed as a Docker container.

## Features

- 🚀 **DNS-based Configuration**: Automatically resolves MAC and IP addresses from DNS TXT records
- 🐳 **Docker Ready**: Fully containerized with multi-stage Docker builds
- ⚡ **Lightweight**: Minimal footprint with .NET 8 runtime
- 🔧 **Flexible Configuration**: Supports environment variables and command-line arguments
- 🌐 **Network Broadcast**: Sends UDP broadcast packets for reliable wake-up functionality
- 📦 **Cross-platform**: Runs on any platform supporting .NET 8

## How It Works

MagicPacket uses the Wake-on-LAN protocol to remotely wake up network devices. It:

1. Queries DNS TXT records for the target domain to retrieve MAC and IP addresses
2. Constructs a Magic Packet containing the target device's MAC address
3. Broadcasts the packet via UDP to wake up the target device

The Magic Packet format consists of:
- 6 bytes of 0xFF (synchronization stream)
- 16 repetitions of the target MAC address (96 bytes)

## Installation

### Using Docker (Recommended)

```bash
# Build the Docker image
docker build -t magicpacket .

# Run with environment variable
docker run -e MAGICPACKET_DNS=your.wol.domain.com magicpacket

# Run with command-line argument
docker run magicpacket your.wol.domain.com
```

### Manual Installation

```bash
# Clone the repository
git clone https://github.com/jussiroine/MagicPacket.git
cd MagicPacket

# Build the application
dotnet build

# Run the application
dotnet run --project MagicPacket [domain]
```

## Configuration

### DNS TXT Record Setup

Configure your DNS TXT record with the following format:

```
your.wol.domain.com. IN TXT "mac=AA:BB:CC:DD:EE:FF ip=192.168.1.100"
```

**Parameters:**
- `mac=`: MAC address of the target device (colon-separated format)
- `ip=`: IP address of the target device or broadcast address

### Application Configuration

The application accepts configuration through:

1. **Command-line argument**: Pass the domain as the first argument
   ```bash
   dotnet run your.wol.domain.com
   ```

2. **Environment variable**: Set `MAGICPACKET_DNS`
   ```bash
   export MAGICPACKET_DNS=your.wol.domain.com
   dotnet run
   ```

3. **Default fallback**: Uses `wol.example.com` if no configuration is provided

## Usage Examples

### Docker Compose

```yaml
version: '3.8'
services:
  magicpacket:
    build: .
    environment:
      - MAGICPACKET_DNS=wol.mynetwork.local
    network_mode: host
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: magicpacket
spec:
  replicas: 1
  selector:
    matchLabels:
      app: magicpacket
  template:
    metadata:
      labels:
        app: magicpacket
    spec:
      containers:
      - name: magicpacket
        image: magicpacket:latest
        env:
        - name: MAGICPACKET_DNS
          value: "wol.mynetwork.local"
        hostNetwork: true
```

### Automated Scheduling

Use with cron or systemd timers for scheduled wake-ups:

```bash
# Cron job to wake device every morning at 8 AM
0 8 * * * docker run -e MAGICPACKET_DNS=workstation.wol.local magicpacket
```

## Requirements

- .NET 8.0 Runtime
- Network access to DNS server
- UDP broadcast capability on port 7
- Target device must support Wake-on-LAN and be properly configured

## Target Device Configuration

Ensure your target device is configured for Wake-on-LAN:

1. **BIOS/UEFI Settings**: Enable Wake-on-LAN or Wake-on-PCI
2. **Network Adapter**: Enable "Wake on Magic Packet" in device properties
3. **Power Management**: Allow the device to wake the computer
4. **Network Configuration**: Ensure the device is on the same network segment

## Development

### Building from Source

```bash
# Clone the repository
git clone https://github.com/jussiroine/MagicPacket.git
cd MagicPacket

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests (if available)
dotnet test
```

### Docker Development

```bash
# Build development image
docker build -t magicpacket:dev .

# Run with volume mount for development
docker run -v $(pwd):/src -w /src magicpacket:dev
```

## Troubleshooting

### Common Issues

1. **DNS Resolution Fails**
   - Verify DNS TXT record is properly configured
   - Check network connectivity to DNS server
   - Ensure record format matches expected pattern

2. **Device Doesn't Wake Up**
   - Confirm Wake-on-LAN is enabled in device BIOS/UEFI
   - Check network adapter Wake-on-LAN settings
   - Verify MAC address in DNS record is correct
   - Ensure target device is connected to power

3. **Network Connectivity**
   - Verify application has network access
   - Check firewall rules for UDP port 7
   - Ensure broadcast packets are allowed on network

### Debugging

Enable verbose logging by modifying the application or check DNS resolution manually:

```bash
# Test DNS TXT record resolution
nslookup -type=TXT your.wol.domain.com

# Check network connectivity
ping your.target.device.ip
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [.NET 8](https://dotnet.microsoft.com/)
- Containerized with [Docker](https://www.docker.com/)
- Implements the Wake-on-LAN protocol standard
