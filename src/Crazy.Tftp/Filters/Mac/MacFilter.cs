using System.Net;
using Crazy.Tftp.Config;

namespace Crazy.Tftp.Filters.Mac;

internal class MacFilter : IFilter
{
    public bool HasMatch(IPAddress ipAddress)
    {
        var mac = MacAddress.GetByIPAddress(ipAddress);
        return mac != null && Settings.MacAddresses.Contains(mac);
    }

    public string GetPath(IPAddress ipAddress, string path)
    {
        var mac = MacAddress.GetByIPAddress(ipAddress);
        return mac == null ? Path.Combine(Settings.Root, path) : Path.Combine(Settings.Root, mac.ToLower(), path);
    }
}