using System.Net;
using Microsoft.Extensions.Logging;
using Tftp.Net;

namespace Mac.Tftp;

internal class TftpClientRequestHandler
{
    private readonly PathManager _pathManager;
    private readonly ILogger<TftpClientRequestHandler> _logger;
    public TftpClientRequestHandler() : this(new PathManager())
    {
    }

    private TftpClientRequestHandler(PathManager pathManager)
    {
        _pathManager = pathManager;
        _logger = Logger.Get<TftpClientRequestHandler>();
    }

    public void Write(ITftpTransfer transfer, EndPoint client)
    {
        var file = Path.Combine(_pathManager.GetPath((client as IPEndPoint)!.Address), transfer.Filename);

        if (File.Exists(file))
        {
            CancelTransfer(transfer, TftpErrorPacket.FileAlreadyExists);
        }
        else
        {
            OutputTransferStatus(transfer, "Accepting write request from " + client);
            StartTransfer(transfer, new FileStream(file, FileMode.CreateNew));
        }
    }

    public void Read(ITftpTransfer transfer, EndPoint client)
    {
        var file = new FileInfo(Path.Combine(_pathManager.GetPath((client as IPEndPoint)!.Address), transfer.Filename));

        //Is the file within the server directory?
        if (!_pathManager.IsInAllowedPath(file))
        {
            CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
        }
        else if (!file.Exists)
        {
            _logger.LogInformation("File Not Found: {FullName}", file.FullName);
            CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
        }
        else
        {
            OutputTransferStatus(transfer, "Accepting request from " + client);
            StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
        }
    }

    private void StartTransfer(ITftpTransfer transfer, Stream stream)
    {
        transfer.OnProgress += (clientTransfer, progress) => { OutputTransferStatus(clientTransfer, "Progress " + progress); };
        transfer.OnError += (clientTransfer, error) => { OutputTransferStatus(clientTransfer, "Error: " + error); };
        transfer.OnFinished += clientTransfer => { OutputTransferStatus(clientTransfer, "Finished"); };
        transfer.Start(stream);
    }

    private void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
    {
        OutputTransferStatus(transfer, "Cancelling transfer: " + reason.ErrorMessage);
        transfer.Cancel(reason);
    }

    private void OutputTransferStatus(ITftpTransfer transfer, string message)
    {
        _logger.LogInformation("[{Filename}] {Message}]", transfer.Filename, message);
    }
}