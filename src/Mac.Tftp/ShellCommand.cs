using System.Diagnostics;

namespace Mac.Tftp;

internal class ShellCommand
{
    private readonly Dictionary<string, string> EnvironmentVariables = new();
    private readonly string Script;

    public ShellCommand(string script)
    {
        Script = script;
    }

    public string Result { get; private set; } = "";

    public void SetEnvironmentVariable(string key, string value)
    {
        EnvironmentVariables[key] = value;
    }

    public bool Run()
    {
        using var myProcess = new Process();

        myProcess.StartInfo.RedirectStandardOutput = true;
        myProcess.StartInfo.RedirectStandardError = true;
        myProcess.StartInfo.UseShellExecute = false;
        myProcess.StartInfo.CreateNoWindow = false;
        myProcess.StartInfo.FileName = Script;

        foreach ((var key, var value) in EnvironmentVariables)
        {
            myProcess.StartInfo.EnvironmentVariables[key] = value;
        }

        //* Set your output and error (asynchronous) handlers
        myProcess.OutputDataReceived += OutputHandler;
        myProcess.ErrorDataReceived += OutputHandler;

        _ = myProcess.Start();
        myProcess.BeginOutputReadLine();
        myProcess.BeginErrorReadLine();
        myProcess.WaitForExit();

        myProcess.OutputDataReceived -= OutputHandler;
        myProcess.ErrorDataReceived -= OutputHandler;

        return myProcess.ExitCode == 0;
    }

    private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (string.IsNullOrEmpty(outLine.Data))
        {
            return;
        }

        Result += !string.IsNullOrEmpty(Result) ? Environment.NewLine + outLine.Data : outLine.Data;
    }
}