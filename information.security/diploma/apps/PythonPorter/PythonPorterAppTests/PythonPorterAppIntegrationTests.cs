using NUnit.Framework;
using PythonPorterAppTests.TestUtils;

namespace PythonPorterAppTests
{
    [TestFixture]
    public class PythonPorterAppIntegrationTests
    {
        [Test]
        public void Check()
        {
            ExecutionResult result = ExecutionHelper.Execute("");
            Assert.That(result.ExitCode, Is.EqualTo(0));
        }
    }
}
