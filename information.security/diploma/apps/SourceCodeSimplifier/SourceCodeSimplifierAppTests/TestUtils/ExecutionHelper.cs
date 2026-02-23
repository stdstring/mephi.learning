using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SourceCodeSimplifierAppTests.TestUtils
{
    internal record ExecutionResult(Int32 ExitCode, String OutputData, String ErrorData);

    internal static class ExecutionHelper
    {
        public static ExecutionResult Execute(String arguments)
        {
            using (Process utilProcess = new Process())
            {
                String currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
                utilProcess.StartInfo.FileName = Path.Combine(currentDir, UtilFilename);
                utilProcess.StartInfo.Arguments = arguments;
                utilProcess.StartInfo.UseShellExecute = false;
                utilProcess.StartInfo.CreateNoWindow = true;
                utilProcess.StartInfo.RedirectStandardError = true;
                utilProcess.StartInfo.RedirectStandardOutput = true;
                utilProcess.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                utilProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                utilProcess.StartInfo.WorkingDirectory = currentDir;
                IList<String> output = new List<String>();
                IList<String> error = new List<String>();
                utilProcess.OutputDataReceived += (_, e) =>
                {
                    if (e.Data != null)
                        output.Add(e.Data);
                };
                utilProcess.ErrorDataReceived += (_, e) =>
                {
                    if (e.Data != null)
                        error.Add(e.Data);
                };
                utilProcess.Start();
                utilProcess.BeginErrorReadLine();
                utilProcess.BeginOutputReadLine();
                utilProcess.WaitForExit();
                return new ExecutionResult(utilProcess.ExitCode, String.Join("\r\n", output), String.Join("\r\n", error));
            }
        }

        private const String UtilFilename = "SourceCodeSimplifierApp.exe";
    }
}