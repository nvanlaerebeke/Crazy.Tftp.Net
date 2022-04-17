using System.Collections.Concurrent;
using System.Net;

namespace Mac.Tftp;

internal static class MacAddress
{
    private static readonly ConcurrentDictionary<string, string> _macAddressCache = new();

    /// <summary>
    /// Gets the MAC address hex format based on the IP address
    ///
    /// Note that the IP address must already be in the ARP cache list
    /// 
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public static string GetByIPAddress(IPAddress ipAddress)
    {
        if (
            _macAddressCache.ContainsKey(ipAddress.ToString()) &&
            _macAddressCache.TryGetValue(ipAddress.ToString(), out var mac))
        {
            return mac;
        }

        var macAddress = GetMacAddress(ipAddress);
        _macAddressCache.TryAdd(ipAddress.ToString(), macAddress);
        return macAddress;
    }

    private static string GetMacAddress(IPAddress address)
    {
        var c = new ShellCommand("getMac.sh");
        c.SetEnvironmentVariable("IP", address.ToString());
        if (c.Run())
        {
            return c.Result;
        }

        throw new Exception("Failed getting MAC address");
    }
}