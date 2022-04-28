using System.Collections.Concurrent;
using System.Net;
using Crazy.Tftp.Config;
using Crazy.Tftp.Filters;
using Microsoft.Extensions.Logging;
using Tftp.Net;

namespace Crazy.Tftp;

internal class TftpClientRequestHandler
{
    private readonly List<IFilter> _filters = new();
    private readonly ConcurrentDictionary<string, string> _progress = new();
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
            transfer.Cancel(TftpErrorPacket.FileNotFound);
            Log.LogInformation("[WRITE]{path} not found", transfer.Filename);
            return;
        }

        var file = new FileInfo(path);
        if (!file.FullName.StartsWith(Settings.Root, StringComparison.InvariantCultureIgnoreCase))
        {
            transfer.Cancel(TftpErrorPacket.AccessViolation);
            Log.LogInformation("[WRITE]Access denied on {path}", path);
            return;
        }

        if (!file.Exists)
        {
            transfer.Cancel(TftpErrorPacket.FileAlreadyExists);
            Log.LogInformation("[WRITE]{path} already exists", path);
            return;
        }

        StartTransfer(transfer, new FileStream(file.FullName, FileMode.CreateNew));
    }

    public void Read(ITftpTransfer transfer, EndPoint client)
    {
        var path = GetPath(transfer, (client as IPEndPoint)!.Address);
        if (string.IsNullOrEmpty(path))
        {
            transfer.Cancel(TftpErrorPacket.FileNotFound);
            Log.LogInformation("[READ]{path} not found", transfer.Filename);
            return;
        }

        var file = new FileInfo(path);
        if (!file.FullName.StartsWith(Settings.Root, StringComparison.InvariantCultureIgnoreCase))
        {
            transfer.Cancel(TftpErrorPacket.AccessViolation);
            Log.LogInformation("[READ]{path} access denied", path);
            return;
        }

        if (!file.Exists)
        {
            transfer.Cancel(TftpErrorPacket.FileNotFound);
            Log.LogInformation("[READ]{path} not found", path);
            return;
        }

        Log.LogInformation("Starting download for {file}", transfer.Filename);
        StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
    }

    private void StartTransfer(ITftpTransfer transfer, Stream stream)
    {
        transfer.OnProgress += TransferOnOnProgress;
        transfer.OnError += TransferOnOnError;
        transfer.OnFinished += TransferOnOnFinished;
        transfer.Start(stream);
    }

    private void TransferOnOnFinished(ITftpTransfer transfer)
    {
        transfer.OnProgress -= TransferOnOnProgress;
        transfer.OnError -= TransferOnOnError;
        transfer.OnFinished -= TransferOnOnFinished;

        _progress.TryRemove(transfer.Filename, out _);
        Log.LogInformation("Finished {file}", transfer.Filename);
    }

    private void TransferOnOnError(ITftpTransfer transfer, TftpTransferError error)
    {
        Log.LogError("Error during transfer for {file}: {error}", transfer.Filename, error);
    }

    private void TransferOnOnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
    {
        if (_progress.TryGetValue(transfer.Filename, out var oldProgress))
        {
            if (oldProgress != progress.ToString())
            {
                Log.LogInformation("[{file}]: {progress} ", transfer.Filename, progress.ToString());
            }
        }
        else
        {
            Log.LogInformation("[{file}]: {progress} ", transfer.Filename, progress.ToString());
        }

        _progress.AddOrUpdate(transfer.Filename, progress.ToString(), (_, _) => progress.ToString());
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
}