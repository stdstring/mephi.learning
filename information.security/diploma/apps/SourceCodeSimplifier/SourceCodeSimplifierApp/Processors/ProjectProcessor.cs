using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierApp.Utils;

namespace SourceCodeSimplifierApp.Processors
{
    internal class ProjectProcessor : ISourceProcessor
    {
        public ProjectProcessor(String? projectFilename, IOutput output)
        {
            if (String.IsNullOrEmpty(projectFilename))
                throw new ArgumentNullException(nameof(projectFilename));
            if (!File.Exists(projectFilename))
                throw new ArgumentException($"Bad (unknown) target {projectFilename}");
            _projectFilename = Path.GetFullPath(projectFilename);
            _output = output;
        }

        public void Process(IList<ITransformer> transformers)
        {
            _output.WriteInfoLine($"Processing of the project {_projectFilename} is started");
            DotnetUtilityService.Build(_projectFilename, _output);
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project sourceProject = workspace.OpenProjectAsync(_projectFilename).Result;
            Project destProject = Process(sourceProject, transformers);
            workspace.TryApplyChanges(destProject.Solution);
            _output.WriteInfoLine($"Processing of the project {_projectFilename} is finished");
        }

        private Project Process(Project sourceProject, IList<ITransformer> transformers)
        {
            if (sourceProject.FilePath == null)
                throw new InvalidOperationException("Bad project (without defined path)");
            Compilation? compilation = sourceProject.GetCompilationAsync().Result;
            if (compilation == null)
                throw new InvalidOperationException("Empty (null) compilation");
            if (!CompilationChecker.CheckCompilationErrors(sourceProject.FilePath, compilation, _output))
                throw new InvalidOperationException("Bad compilation (with errors)");
            DocumentId[] documentIds = sourceProject.Documents
                .Where(doc => doc.SourceCodeKind == SourceCodeKind.Regular)
                .Select(doc => doc.Id)
                .ToArray();
            Project destProject = sourceProject;
            foreach (DocumentId documentId in documentIds)
            {
                Document? sourceDocument = destProject.GetDocument(documentId);
                if (sourceDocument == null)
                    throw new InvalidOperationException($"Unknown document with id {documentId}");
                Document destDocument = Process(sourceDocument, transformers);
                destProject = destDocument.Project;
            }
            return destProject;
        }

        private Document Process(Document sourceDocument, IList<ITransformer> transformers)
        {
            if (sourceDocument.FilePath == null)
                throw new InvalidOperationException("Bad document (without defined path)");
            if (ProjectIgnoredFiles.IgnoreFile(sourceDocument.FilePath))
                return sourceDocument;
            String filePath = sourceDocument.FilePath;
            Document destDocument = sourceDocument;
            _output.WriteInfoLine($"Processing of the file {filePath} is started");
            foreach (ITransformer transformer in transformers)
                destDocument = transformer.Transform(destDocument);
            _output.WriteInfoLine($"Processing of the file {filePath} is finished");
            return destDocument;
        }

        private readonly String _projectFilename;
        private readonly IOutput _output;
    }
}
