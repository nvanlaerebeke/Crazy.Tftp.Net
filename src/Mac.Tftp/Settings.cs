using System.Reflection;
using System.Text.Json;

namespace Mac.Tftp;

internal static class Settings
{
    public static List<string> MacAddresses
    {
        get
        {
            var json = Environment.GetEnvironmentVariable("MAC_ADDRESSES");
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("MAC_ADDRESSES environment variable not set");
            }

            var list = JsonSerializer.Deserialize<List<string>>(json);
            list = list?.ConvertAll(i => i.ToLower());
            return list ?? new List<string>();
        }
    }

    public static string Root
    {
        get
        {
            var root = Environment.GetEnvironmentVariable("TFTP_ROOT");
            return string.IsNullOrEmpty(root) ? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "data") : root;
        }
    }


    public static string DefaultPath
    {
        get
        {
            var defaultPath = Environment.GetEnvironmentVariable("TFTP_DEFAULT_ROOT");
            return string.IsNullOrEmpty(defaultPath) ? Root : defaultPath;
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