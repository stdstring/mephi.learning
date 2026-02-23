using NUnit.Framework;
using SourceCodeSimplifierApp.Variables;

namespace SourceCodeSimplifierAppTests.Variables
{
    [TestFixture]
    public class VariableGeneratorTests
    {
        [Test]
        public void Generate()
        {
            ISet<String> knownVariables = new HashSet<String>(new[]{"param", "param3", "item", "item2", "var2", "data"});
            VariableGenerator generator = new VariableGenerator(knownVariables);
            Assert.That(generator.Generate("portion"), Is.EqualTo("portion"));
            Assert.That(generator.Generate("portion"), Is.EqualTo("portion2"));
            Assert.That(generator.Generate("portion"), Is.EqualTo("portion3"));
            Assert.That(generator.Generate("param"), Is.EqualTo("param2"));
            Assert.That(generator.Generate("param"), Is.EqualTo("param4"));
            Assert.That(generator.Generate("param"), Is.EqualTo("param5"));
            Assert.That(generator.Generate("item"), Is.EqualTo("item3"));
            Assert.That(generator.Generate("var"), Is.EqualTo("var"));
            Assert.That(generator.Generate("var"), Is.EqualTo("var3"));
            Assert.That(generator.Generate("var"), Is.EqualTo("var4"));
        }
    }
}
