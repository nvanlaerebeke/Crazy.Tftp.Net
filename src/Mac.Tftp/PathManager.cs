using System.Net;

namespace Mac.Tftp;

internal class PathManager
{
    private readonly string _defaultPath;
    private readonly List<string> _macAddresses;
    private readonly string _root;

    public PathManager()
    {
        _macAddresses = Settings.MacAddresses;
        _root = Settings.Root;
        _defaultPath = Settings.DefaultPath;
    }

    public string GetPath(IPAddress ipAddress)
    {
        var mac = MacAddress.GetByIPAddress(ipAddress);
        return _macAddresses.Contains(mac.ToLower()) ? Path.Combine(_root, mac) : _defaultPath;
    }

    public bool IsInAllowedPath(FileInfo file)
    {
        return file.FullName.StartsWith(_root, StringComparison.InvariantCultureIgnoreCase) ||
               file.FullName.StartsWith(_defaultPath, StringComparison.InvariantCultureIgnoreCase);
    }
}