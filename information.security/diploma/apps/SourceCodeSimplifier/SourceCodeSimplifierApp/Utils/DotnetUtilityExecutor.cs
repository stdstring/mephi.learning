using System.Diagnostics;
using SourceCodeSimplifierApp.Output;

namespace SourceCodeSimplifierApp.Utils
{
    internal record AppExecuteResult(int ExitCode, string[] Output, string[] Error);

    internal static class DotnetUtilityExecutor
    {
        public static AppExecuteResult Build(String path)
        {
            IList<String> output = new List<String>();
            IList<String> error = new List<String>();
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"build {path}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += delegate (Object _, DataReceivedEventArgs e)
                {
                    if (e.Data != null)
                        output.Add($"{e.Data}");
                };
                process.ErrorDataReceived += delegate (Object _, DataReceivedEventArgs e)
                {
                    if (e.Data != null)
                        error.Add($"{e.Data}");
                };
                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    return new AppExecuteResult(ExitCode: process.ExitCode, Output: output.ToArray(), Error: error.ToArray());
                }
                catch (Exception ex)
                {
                    error.Add($"{ex.Message}");
                    return new AppExecuteResult(ExitCode: -1, Output: output.ToArray(), Error: error.ToArray());
                }
            }
        }
    }

    internal static class DotnetUtilityService
    {
        public static void Build(String path, IOutput output)
        {
            AppExecuteResult buildResult = DotnetUtilityExecutor.Build(path);
            if (buildResult.ExitCode == 0)
                output.WriteInfoLine("dotnet build is succeeded");
            else
            {
                output.WriteFailLine("dotnet build is failed due to the following reason:");
                foreach (String errorLine in buildResult.Error)
                    output.WriteFailLine(errorLine);
                throw new InvalidOperationException("dotnet build is failed");
            }
        }
    }
}