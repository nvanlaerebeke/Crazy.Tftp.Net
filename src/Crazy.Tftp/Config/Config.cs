using System.Text.Json.Serialization;

namespace Crazy.Tftp.Config;

/// <summary>
///     Represents the json config file for Crazy.Tftp
/// </summary>
internal class Config
{
    [JsonPropertyName("data_location")]
    public string DataLocation { get; set; } = "/var/lib/crazy.tftp/data";

    [JsonPropertyName("port")]
    public int Port { get; set; } = 69;

    [JsonPropertyName("mac_filter")]
    public List<string> MACFilter { get; set; } = new();

    [JsonPropertyName("ip_filter")]
    public List<string> IPFilter { get; set; } = new();
}