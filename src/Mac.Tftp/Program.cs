using Mac.Tftp;

AutoResetEvent Closing = new(false);

Console.CancelKeyPress += (_, _) =>
{
    Console.WriteLine("Exit received...");
    Closing.Set();
};

//Start the server
using var server = new MacTftpServer();
server.Start();

Closing.WaitOne();