using Microsoft.CodeAnalysis;
using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;

namespace SourceCodeSimplifierAppTests.TestUtils
{
    internal class TransformerHelper
    {
        public TransformerHelper(String source, String namePrefix, OutputLevel outputLevel)
        {
            _source = source;
            _namePrefix = namePrefix;
            _namePrefixDest = $"{namePrefix}Dest";
            _outputLevel = outputLevel;
        }

        public void Process(Func<IOutput, ITransformer> transformerFactory, String expectedOutput, String expectedResult)
        {
            Document sourceDocument = PreparationHelper.Prepare(_source, _namePrefix);
            using (TextWriter outputWriter = new StringWriter())
            using (TextWriter errorWriter = new StringWriter())
            {
                IOutput output = new OutputImpl(outputWriter, errorWriter, _outputLevel);
                ITransformer transformer = transformerFactory(output);
                Document destDocument = ProcessImpl(sourceDocument, transformer);
                String actualOutput = outputWriter.ToString() ?? "";
                String actualError = errorWriter.ToString() ?? "";
                String actualResult = destDocument.GetTextAsync().Result.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(actualResult, Is.EqualTo(expectedResult));
                    Assert.That(actualOutput, Is.EqualTo(expectedOutput));
                    Assert.That(actualError, Is.Empty);
                });
            }
        }

        public void Process(Func<IOutput, ITransformer[]> transformersFactory, String expectedOutput, String expectedResult)
        {
            Document sourceDocument = PreparationHelper.Prepare(_source, _namePrefix);
            using (TextWriter outputWriter = new StringWriter())
            using (TextWriter errorWriter = new StringWriter())
            {
                IOutput output = new OutputImpl(outputWriter, errorWriter, _outputLevel);
                Document destDocument = transformersFactory(output).Aggregate(sourceDocument, ProcessImpl);
                String actualOutput = outputWriter.ToString() ?? "";
                String actualError = errorWriter.ToString() ?? "";
                String actualResult = destDocument.GetTextAsync().Result.ToString();
                Assert.Multiple(() =>
                {
                    Assert.That(actualResult, Is.EqualTo(expectedResult));
                    Assert.That(actualOutput, Is.EqualTo(expectedOutput));
                    Assert.That(actualError, Is.Empty);
                });
            }
        }

        public void ProcessWithException<TException>(Func<IOutput, ITransformer> transformerFactory) where TException : Exception
        {
            Document sourceDocument = PreparationHelper.Prepare(_source, _namePrefix);
            IOutput output = new NullOutput();
            ITransformer transformer = transformerFactory(output);
            Assert.Throws<TException>(() => transformer.Transform(sourceDocument));
        }

        private Document ProcessImpl(Document sourceDocument, ITransformer transformer)
        {
            Document destDocument = transformer.Transform(sourceDocument);
            PreparationHelper.CheckCompilationErrors(destDocument, _namePrefixDest);
            return destDocument;
        }

        private readonly String _source;
        private readonly String _namePrefix;
        private readonly String _namePrefixDest;
        private readonly OutputLevel _outputLevel;
    }
}
