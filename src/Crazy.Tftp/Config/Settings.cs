using System.Text.Json;

namespace Crazy.Tftp.Config;

internal static class Settings
{
    /// <summary>
    /// Configuration file is only read out once and is cached in this variable
    /// </summary>
    private static Config? _config;

    private const string CONFIGURATION_FILE = "/etc/crazy.tftp";
    private const string DATA_DIRECTORY = "/var/lib/crazy.tftp/data";

    public static List<string> MacAddresses => GetConfig().MACFilter;

    public static List<string> IpAddresses => GetConfig().IPFilter;

    /// <summary>
    ///     If the "data_location is filled in in the /etc/crazy.tftp config file that is ued.
    ///     If not the TFTP_ROOT environment variable is used.
    ///     If that is not set the default is /var/lib/crazy.tftp/data
    /// </summary>
    public static string Root
    {
        get
        {
            var root = GetConfig().DataLocation;
            return !string.IsNullOrEmpty(root) ? root : DATA_DIRECTORY;
        }
    }

    /// <summary>
    ///     Port to run the tftp server on
    ///     69 by default
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static int Port => GetConfig().Port;

    /// <summary>
    ///     Reads out the configure file located at /etc/crazy.tftp
    /// </summary>
    /// <returns></returns>
    private static Config GetConfig()
    {
        if (_config != null)
        {
            return _config;
        }

        if (!File.Exists(CONFIGURATION_FILE))
        {
            return new Config();
        }

        _config = JsonSerializer.Deserialize<Config>(File.ReadAllText(CONFIGURATION_FILE));
        return _config ?? new Config();
    }
}