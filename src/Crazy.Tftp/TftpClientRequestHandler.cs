using System.Net;
using Crazy.Tftp.Filters;
using Crazy.Tftp.Filters.Mac;
using Microsoft.Extensions.Logging;
using Tftp.Net;

namespace Crazy.Tftp;

internal class TftpClientRequestHandler
{
    private readonly List<IFilter> _filters = new();
    private readonly ILogger Log;
    
    public TftpClientRequestHandler()
    {
        _filters.Add(new DefaultFilter());
        Log = Logger.Get<TftpClientRequestHandler>();
    }

    public void AddFilter(IFilter filter)
    {
        _filters.Add(filter);
    }
    
    public void Write(ITftpTransfer transfer, EndPoint client)
    {
        var path = GetPath(transfer, (client as IPEndPoint)!.Address);

        if (string.IsNullOrEmpty(path))
        {
            CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
        }

        var file = new FileInfo(path);
        if (!file.FullName.StartsWith(Settings.Root, StringComparison.InvariantCultureIgnoreCase))
        {
            CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
        }
        
        if(file.Exists)
        {
            CancelTransfer(transfer, TftpErrorPacket.FileAlreadyExists);
        }
        StartTransfer(transfer, new FileStream(file.FullName, FileMode.CreateNew));
    }

    public void Read(ITftpTransfer transfer, EndPoint client)
    {
        var path = GetPath(transfer, (client as IPEndPoint)!.Address);
        if (string.IsNullOrEmpty(path))
        {
            CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
        }
        
        var file = new FileInfo(path);
        if (!file.FullName.StartsWith(Settings.Root, StringComparison.InvariantCultureIgnoreCase))
        {
            CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
        }

        if (!file.Exists)
        {
            CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
        }
        StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
    }

    private void StartTransfer(ITftpTransfer transfer, Stream stream)
    {
        transfer.OnProgress += (_, _) => { };
        transfer.OnError += (clientTransfer, error) =>
        {
            Log.LogError("Error during transfer for {file}: {error}", clientTransfer.Filename, error);
        };
        transfer.OnFinished += clientTransfer =>
        {
            Log.LogInformation("Finished {file}", clientTransfer.Filename);

        };
        Log.LogInformation("Starting download for {file}", transfer.Filename);
        transfer.Start(stream);
    }
    private string GetPath(ITftpTransfer transfer, IPAddress ipAddress)
    {
        var s = "";
        _filters.ForEach(f =>
        {
            if (f.HasMatch(ipAddress))
            {
                s = f.GetPath(ipAddress, transfer.Filename);
            }
        });
        return s;
    }
    
    private void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
    {
        transfer.Cancel(reason);
    }
}