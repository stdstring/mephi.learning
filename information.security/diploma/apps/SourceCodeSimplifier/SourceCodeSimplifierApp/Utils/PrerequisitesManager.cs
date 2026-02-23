using Microsoft.Build.Locator;

namespace SourceCodeSimplifierApp.Utils
{
    internal static class PrerequisitesManager
    {
        public static void Run()
        {
            // usage of MSBuild
            MSBuildLocator.RegisterDefaults();
        }
    }
}
