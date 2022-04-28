using System.Net;
using Crazy.Tftp.Config;
using Crazy.Tftp.Filters;

namespace Crazy.Tftp;

internal class TftpServer : IDisposable
{
    private readonly TftpClientRequestHandler _tftpClientRequestHandler;
    private readonly global::Tftp.Net.TftpServer _tftpServer;

    public TftpServer() : this(new TftpClientRequestHandler())
    {
    }

    private TftpServer(TftpClientRequestHandler tftpClientRequestHandler)
    {
        _tftpClientRequestHandler = tftpClientRequestHandler;
        _tftpServer = new global::Tftp.Net.TftpServer(IPAddress.Any, Settings.Port);
    }

    public void Start()
    {
        _tftpServer.OnReadRequest += (transfer, client) => { _tftpClientRequestHandler.Read(transfer, client); };
        _tftpServer.OnWriteRequest += (transfer, client) => { _tftpClientRequestHandler.Write(transfer, client); };
        _tftpServer.Start();
    }

    public void AddFilter(IFilter filter)
    {
        _tftpClientRequestHandler.AddFilter(filter);
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

    ~TftpServer()
    {
        Dispose(false);
    }

    #endregion
}