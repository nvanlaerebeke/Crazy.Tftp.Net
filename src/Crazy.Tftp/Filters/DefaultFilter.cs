using System.Net;
using Crazy.Tftp.Config;

namespace Crazy.Tftp.Filters;

internal class DefaultFilter : IFilter
{
    public bool HasMatch(IPAddress ipAddress)
    {
        return true;
    }

    public string GetPath(IPAddress ipAddress, string path)
    {
        return Path.Combine(Settings.Root, path);
    }
}