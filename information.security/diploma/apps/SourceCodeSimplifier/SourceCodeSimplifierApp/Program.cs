using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Processors;
using SourceCodeSimplifierApp.Utils;
using System.Text;
using SourceCodeSimplifierApp.Transformers;

namespace SourceCodeSimplifierApp
{
    internal class Program
    {
        static Int32 Main(string[] args)
        {
            try
            {
                return MainImpl(args) ? 0 : -1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"[ERROR]: {e.Message}");
                return -1;
            }
        }

        private static Boolean MainImpl(String[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            switch (AppArgsParser.Parse(args))
            {
                case AppArgsResult.VersionConfig {Version: var version}:
                    Console.WriteLine(version);
                    return true;
                case AppArgsResult.HelpConfig {Help: var help}:
                    Console.WriteLine(help);
                    return true;
                case AppArgsResult.WrongConfig {Help: var help, Reason: var reason}:
                    Console.Error.WriteLine($"[ERROR]: {reason}");
                    Console.WriteLine(help);
                    return false;
                case AppArgsResult.MainConfig mainConfig:
                    ProcessTransform(mainConfig);
                    return true;
                default:
                    throw new InvalidOperationException("Unsupported args");
            }
        }

        private static void ProcessTransform(AppArgsResult.MainConfig mainConfig)
        {
            AppConfig appConfig = AppConfigFactory.Create(mainConfig);
            IOutput output = new OutputImpl(Console.Out, Console.Error, appConfig.Config.BaseConfig!.OutputLevel);
            PrerequisitesManager.Run();
            ISourceProcessor processor = SourceProcessorFactory.Create(appConfig.Config.BaseConfig.Target!, output);
            IList<ITransformer> transformers = TransformersFactory.Create(output, appConfig.Config.Transformers ?? Array.Empty<TransformerEntry>());
            processor.Process(transformers);
        }
    }
}
