using System.Net;
using Crazy.Tftp;
using Crazy.Tftp.Config;
using Crazy.Tftp.Filters.IP;
using Crazy.Tftp.Filters.Mac;
using Microsoft.Extensions.Logging;

AutoResetEvent Closing = new(false);

Console.CancelKeyPress += (_, _) =>
{
    Console.WriteLine("Exit received...");
    Closing.Set();
};

var log = Logger.Get<Program>();

log.LogInformation("Listening on {ip}:{port}", IPAddress.Any, Settings.Port);
log.LogInformation("Document Root: {root}", Settings.Root);

if (!Directory.Exists(Settings.Root))
{
    log.LogInformation("Data directory '{data_dir}' does not exist, creating it...", Settings.Root);
    Directory.CreateDirectory(Settings.Root);
}

//Tftp Server Instance
using var server = new TftpServer();

//Mac Addresses
if (Settings.MacAddresses.Count > 0)
{
    log.LogInformation("Found {count} Mac Addresses:", Settings.MacAddresses.Count);
    Settings.MacAddresses.ForEach(mac => log.LogInformation("-> {mac}", mac));
    server.AddFilter(new MacFilter());
}

//IP Addresses
if (Settings.IpAddresses.Count > 0)
{
    log.LogInformation("Found {count} Ip Addresses:", Settings.IpAddresses.Count);
    Settings.IpAddresses.ForEach(ip => log.LogInformation("-> {ip}", ip));
    server.AddFilter(new IpFilter());
}

server.Start();
Closing.WaitOne();