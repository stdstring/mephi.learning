using NUnit.Framework;
using SimplePythonPorter.Common;
using SimplePythonPorter.Processor;

namespace SimplePythonPorter
{
    [TestFixture]
    public class ExperimentExample1TransformTest
    {
        [Test]
        public void Check()
        {
            const String experimentExample1Project = "..\\..\\..\\..\\ExperimentExample1\\ExperimentExample1.csproj";
            AppData appData = new AppData(Results: new List<TransformResult>());
            ProjectProcessor processor = new ProjectProcessor(appData);
            processor.Process(experimentExample1Project);
            Assert.That(appData.Results.Count, Is.EqualTo(1));
            Console.WriteLine("\nResult:\n");
            Console.WriteLine(appData.Results[0].Content);
        }
    }
}
