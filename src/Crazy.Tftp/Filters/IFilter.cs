using System.Net;

namespace Crazy.Tftp.Filters;

internal interface IFilter
{
    bool HasMatch(IPAddress ipAddress);
    string GetPath(IPAddress ipAddress, string path);
}