using System.Net;

namespace Crazy.Tftp.Filters.IP;

internal class IpFilter : IFilter
{
    public bool HasMatch(IPAddress ipAddress)
    {
        return Settings.IpAddresses.Contains(ipAddress.ToString());
    }

    public string GetPath(IPAddress ipAddress, string path)
    {
        return Path.Combine(Settings.Root, ipAddress.ToString(), path);
    }
}