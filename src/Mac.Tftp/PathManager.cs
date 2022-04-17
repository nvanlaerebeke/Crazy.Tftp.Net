using System.Net;
using Microsoft.Extensions.Logging;

namespace Mac.Tftp;

internal class PathManager
{
    private readonly string _defaultPath;
    private readonly List<string> _macAddresses;
    private readonly string _root;
    private readonly ILogger<PathManager> _logger;
    
    public PathManager()
    {
        _macAddresses = Settings.MacAddresses;
        _root = Settings.Root;
        _defaultPath = Settings.DefaultPath;
        _logger = Logger.Get<PathManager>();
        
        _logger.LogInformation("Root: {Root}", _root);
        _logger.LogInformation("DefaultPath: {DefaultPath}:", _defaultPath);
        _logger.LogInformation("Mac Addresses in list:");
        _macAddresses.ForEach(m => _logger.LogInformation("{Mac}", m));
    }

    public string GetPath(IPAddress ipAddress)
    {
        try
        {
            var mac = MacAddress.GetByIPAddress(ipAddress);
            _logger.LogInformation("Request from {Mac}", mac);
            return _macAddresses.Contains(mac.ToLower()) ? Path.Combine(_root, mac) : _defaultPath;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get MAC address for {IpAddress}, falling back to default", ipAddress.ToString());
            _logger.LogError("Reason: {Reason}", ex.Message);
        }
        return _defaultPath;
    }

    public bool IsInAllowedPath(FileInfo file)
    {
        return file.FullName.StartsWith(_root, StringComparison.InvariantCultureIgnoreCase) ||
               file.FullName.StartsWith(_defaultPath, StringComparison.InvariantCultureIgnoreCase);
    }
}