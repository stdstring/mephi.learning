using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SimplePythonPorter.Checker;
using SimplePythonPorter.Common;
using SimplePythonPorter.Utils;

namespace SimplePythonPorter.Processor
{
    internal class ProjectProcessor
    {
        public ProjectProcessor(AppData appData)
        {
            _appData = appData;
            _fileProcessor = new FileProcessor(appData);
        }

        public void Process(String projectFilename)
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync(projectFilename).Result;
            ProcessImpl(project);
        }

        private void ProcessImpl(Project project)
        {
            String filePath = project.FilePath.Must("Bad project: without path");
            if(!RestoreNuget(filePath))
                throw new InvalidOperationException();
            Compilation? compilation = project.GetCompilationAsync().Result;
            if (compilation == null)
                throw new InvalidOperationException();
            if (!CompilationChecker.CheckCompilationErrors(filePath, compilation))
                throw new InvalidOperationException();
            String projectDir = Path.GetDirectoryName(project.FilePath).Must("Bad project: path without directory name");
            foreach (Document document in project.Documents.Where(doc => doc.SourceCodeKind == SourceCodeKind.Regular))
            {
                String documentRelativePath = Path.GetRelativePath(projectDir, document.FilePath!);
                _fileProcessor.Process(documentRelativePath, document, compilation);
            }
        }

        private bool RestoreNuget(string projectPath)
        {
            // It seems that the best solution to restore nuget packages is to use "dotnet restore" command.
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"restore {projectPath}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error at restoring nuget: {ex.Message}");
                    return false;
                }
            }
        }

        private readonly AppData _appData;
        private readonly FileProcessor _fileProcessor;
    }
}
