using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.Processor;

namespace SimplePythonPorterApp
{
    internal class Program
    {
        private static void ShowHelp()
        {
            Console.WriteLine("Usage is the following");
            Console.WriteLine("<app> <path-to-source-project>");
        }

        private static void ProcessProject(String projectPath)
        {
            AppData appData = new AppData(NameTransformer: new NameTransformer(),
                                          Results: new List<TransformResult>());
            ProjectProcessor processor = new ProjectProcessor(appData);
            processor.Process(projectPath);
            foreach (TransformResult entry in appData.Results)
            {
                Console.WriteLine();
                Console.WriteLine(entry.RelativePath);
                Console.WriteLine(entry.Content);
            }
        }

        static Int32 Main(string[] args)
        {
            switch (args)
            {
                case []:
                    ShowHelp();
                    return 0;
                case [var path]:
                    ProcessProject(path);
                    return 0;
                default:
                    Console.WriteLine("Bad usage of porter app");
                    ShowHelp();
                    return -1;
            }
        }
    }
}
