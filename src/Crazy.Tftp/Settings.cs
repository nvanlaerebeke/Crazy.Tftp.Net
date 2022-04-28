using System.Text.Json;

namespace Crazy.Tftp;

internal static class Settings
{
    private static List<string>? _macAddresses;
    private static List<string>? _ipAddresses;

    public static List<string> MacAddresses
    {
        get
        {
            if (_macAddresses != null)
            {
                return _macAddresses;
            }

            _macAddresses = new List<string>();

            var json = Environment.GetEnvironmentVariable("MAC_ADDRESSES");
            if (!string.IsNullOrEmpty(json))
            {
                var macAddresses = JsonSerializer.Deserialize<string[]>(json);
                if (macAddresses != null)
                {
                    _macAddresses.AddRange(macAddresses);
                }
            }

            if (File.Exists("/etc/mac.tftp.config"))
            {
                _macAddresses.AddRange(File.ReadAllLines("/etc/mac.tftp.config"));
            }

            _macAddresses = _macAddresses.Distinct().ToList();
            return _macAddresses;
        }
    }

    public static List<string> IpAddresses
    {
        get
        {
            if (_ipAddresses != null)
            {
                return _ipAddresses;
            }

            _ipAddresses = new List<string>();

            var json = Environment.GetEnvironmentVariable("IP_ADDRESSES");
            if (!string.IsNullOrEmpty(json))
            {
                var ips = JsonSerializer.Deserialize<string[]>(json);
                if (ips != null)
                {
                    _ipAddresses.AddRange(ips);
                }
            }

            if (File.Exists("/etc/ip.tftp.config"))
            {
                _ipAddresses.AddRange(File.ReadAllLines("/etc/ip.tftp.config"));
            }

            _ipAddresses = _ipAddresses.Distinct().ToList();
            return _ipAddresses;
        }
    }

    public static string Root
    {
        get
        {
            var root = Environment.GetEnvironmentVariable("TFTP_ROOT");
            if (string.IsNullOrEmpty(root))
            {
                root = "/var/lib/crazy.tftp/data";
            }

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            return root;
        }
    }

    public static int Port
    {
        get
        {
            var port = Environment.GetEnvironmentVariable("PORT");
            if (string.IsNullOrEmpty(port))
            {
                return 69;
            }

            if (int.TryParse(port, out var intPort))
            {
                return intPort;
            }

            throw new Exception("Invalid Port Given");
        }
    }
}