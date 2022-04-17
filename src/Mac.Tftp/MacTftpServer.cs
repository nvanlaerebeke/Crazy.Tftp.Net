using System.Net;
using Microsoft.Extensions.Logging;
using Tftp.Net;

namespace Mac.Tftp;

internal class MacTftpServer : IDisposable
{
    private readonly TftpClientRequestHandler _tftpClientRequestHandler;
    private readonly TftpServer _tftpServer;
    private readonly ILogger<MacTftpServer> _logger;

    public MacTftpServer() : this(new TftpClientRequestHandler())
    {
    }

    private MacTftpServer(TftpClientRequestHandler tftpClientRequestHandler)
    {
        _tftpClientRequestHandler = tftpClientRequestHandler;
        _tftpServer = new TftpServer(IPAddress.Any, Settings.Port);
        _logger = Logger.Get<MacTftpServer>();
    }

    public void Start()
    {
        _logger.LogInformation("Running TFTP server");
        _tftpServer.OnReadRequest += (transfer, client) =>
        {
            _logger.LogInformation("Read Request");
            _tftpClientRequestHandler.Read(transfer, client);
        };
        _tftpServer.OnWriteRequest += (transfer, client) =>
        {
            _logger.LogInformation("Write Request");
            _tftpClientRequestHandler.Write(transfer, client);
        };
        _tftpServer.Start();
    }

    #region IDisposable

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _tftpServer.Dispose();
        }

        _disposed = true;
    }

    ~MacTftpServer()
    {
        Dispose(false);
    }

    #endregion
}