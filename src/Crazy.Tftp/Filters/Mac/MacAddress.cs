using System.Collections.Concurrent;
using System.Net;

namespace Crazy.Tftp.Filters.Mac;

internal static class MacAddress
{
    private static readonly ConcurrentDictionary<string, string?> _macAddressCache = new();

    /// <summary>
    ///     Gets the MAC address hex format based on the IP address
    ///     Note that the IP address must already be in the ARP cache list
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns>mac address as a string or null when not found</returns>
    public static string? GetByIPAddress(IPAddress ipAddress)
    {
        if (
            _macAddressCache.ContainsKey(ipAddress.ToString()) &&
            _macAddressCache.TryGetValue(ipAddress.ToString(), out var mac))
        {
            return mac;
        }

        var macAddress = GetMacAddress(ipAddress);
        _macAddressCache.TryAdd(ipAddress.ToString().ToLower(), macAddress);
        return macAddress;
    }

    private static string? GetMacAddress(IPAddress address)
    {
        var c = new ShellCommand("getMac.sh");
        c.SetEnvironmentVariable("IP", address.ToString());
        return c.Run() ? c.Result : null;
    }
}