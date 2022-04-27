using System.Net;
using Crazy.Tftp;
using Crazy.Tftp.Filters.IP;
using Crazy.Tftp.Filters.Mac;
using Microsoft.Extensions.Logging;

AutoResetEvent Closing = new(false);

Console.CancelKeyPress += (_, _) =>
{
    Console.WriteLine("Exit received...");
    Closing.Set();
};

//Start the server
using var server = new TftpServer();


var log = Logger.Get<Program>();

log.LogInformation("Listening on {ip}:{port}", IPAddress.Any, Settings.Port);
log.LogInformation("Document Root: {root}", Settings.Root);

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
    Settings.MacAddresses.ForEach(ip => log.LogInformation("-> {ip}", ip));
    server.AddFilter(new IpFilter());
}


server.Start();
Closing.WaitOne();