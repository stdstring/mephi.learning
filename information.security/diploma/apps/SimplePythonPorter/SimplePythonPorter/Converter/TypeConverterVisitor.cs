using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Utils;

namespace SimplePythonPorter.Converter
{
    internal class TypeConverterVisitor : CSharpSyntaxWalker
    {
        public TypeConverterVisitor(SemanticModel model, FileStorage currentFile, AppData appData)
        {
            _model = model;
            _currentFile = currentFile;
            _appData = appData;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            INamedTypeSymbol? currentType = _model.GetDeclaredSymbol(node);
            CheckTypeDeclaration(node, currentType);
            String destClassName = _appData.NameTransformer.TransformTypeName(node.Identifier.Text);
            ClassStorage classStorage = _currentFile.CreateClassStorage(destClassName);
            Boolean isAbstract = node.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword));
            if (isAbstract)
            {
                classStorage.AddBaseClass("ABC");
                classStorage.ImportStorage.AddEntity("abc", "ABC");
            }
            IReadOnlyList<BaseTypeSyntax> baseTypes = node.BaseList == null ? Array.Empty<BaseTypeSyntax>() : node.BaseList.Types;
            foreach (BaseTypeSyntax baseType in baseTypes)
            {
                classStorage.AddBaseClass(baseType.Type.ToString());
            }
            ProcessConstructors(CollectConstructors(node), classStorage);
            ProcessMethods(CollectMethods(node), classStorage);
            base.VisitClassDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            INamedTypeSymbol? currentType = _model.GetDeclaredSymbol(node);
            CheckTypeDeclaration(node, currentType);
            String destClassName = _appData.NameTransformer.TransformTypeName(node.Identifier.Text);
            ClassStorage classStorage = _currentFile.CreateClassStorage(destClassName);
            classStorage.AddBaseClass("ABC");
            classStorage.ImportStorage.AddEntity("abc", "ABC");
            IReadOnlyList<BaseTypeSyntax> baseTypes = node.BaseList == null ? Array.Empty<BaseTypeSyntax>() : node.BaseList.Types;
            foreach (BaseTypeSyntax baseType in baseTypes)
            {
                classStorage.AddBaseClass(baseType.Type.ToString());
            }
            ProcessMethods(CollectMethods(node), classStorage);
            base.VisitInterfaceDeclaration(node);
        }

        private void CheckTypeDeclaration(TypeDeclarationSyntax node, INamedTypeSymbol? currentType)
        {
            if (currentType == null)
                throw new InvalidOperationException("Absence of semantic info");
            SyntaxNode? parentDecl = node.Parent;
            if (parentDecl == null)
                throw new InvalidOperationException("Absence of parent");
            if (!parentDecl.IsKind(SyntaxKind.NamespaceDeclaration))
                throw new UnsupportedSyntaxException("Nested types are not supported");
        }

        private IDictionary<String, IList<MethodData>> CollectMethods(TypeDeclarationSyntax type)
        {
            IDictionary<String, IList<MethodData>> storage = new Dictionary<String, IList<MethodData>>();
            MethodDeclarationSyntax[] methods = type.GetMembers<MethodDeclarationSyntax>();
            foreach (MethodDeclarationSyntax method in methods)
            {
                MethodData data = MethodData.Create(type, method, _model, _appData);
                if (storage.ContainsKey(data.DestName))
                {
                    if (storage[data.DestName][0].Symbol.IsStatic != data.Symbol.IsStatic)
                        throw new UnsupportedSyntaxException($"Unsupported overloading between static and instance methods named \"{data.Symbol.Name}\"");
                    storage[data.DestName].Add(data);
                }
                else
                    storage.Add(data.DestName, new List<MethodData>{data});
            }
            foreach (String methodName in storage.Keys)
            {
                storage[methodName] = SortMethods(storage[methodName]);
            }

            return storage;
        }

        private IList<MethodData> CollectConstructors(TypeDeclarationSyntax type)
        {
            IList<MethodData> dest = type
                .GetMembers<ConstructorDeclarationSyntax>()
                .Select(constructor => MethodData.Create(type, constructor, _model, _appData))
                .ToList();
            return SortMethods(dest);
        }

        private IList<MethodData> SortMethods(IList<MethodData> source)
        {
            // processing overloading
            List<MethodData> dest = new List<MethodData>(source);
            dest.Sort((MethodData left, MethodData right) =>
            {
                if (left.Symbol.Parameters.Length < right.Symbol.Parameters.Length)
                    return -1;
                if (left.Symbol.Parameters.Length > right.Symbol.Parameters.Length)
                    return 1;
                return 0;
            });
            return dest;
        }

        private void ProcessConstructors(IList<MethodData> constructors, ClassStorage classStorage)
        {
            switch (constructors.Count)
            {
                case 0:
                    ProcessDefaultConstructor(classStorage);
                    break;
                case 1:
                    ProcessNonOverloadingConstructor(constructors[0], classStorage);
                    break;
                case > 1:
                    ProcessOverloadingConstructor(constructors, classStorage);
                    break;
            }
        }

        private void ProcessDefaultConstructor(ClassStorage classStorage)
        {
            String[] parameters = new[] {PythonSpecificDef.SelfArg};
            MethodStorage methodStorage = classStorage.CreateMethodStorage(PythonSpecificDef.Constructor, parameters);
        }

        private void ProcessNonOverloadingConstructor(MethodData data, ClassStorage classStorage)
        {
            String[] parameters = data
                .Symbol
                .Parameters
                .Select(parameter => parameter.Name)
                .ToArray();
            parameters = new[] {PythonSpecificDef.SelfArg}.Concat(parameters).ToArray();
            MethodStorage methodStorage = classStorage.CreateMethodStorage(data.DestName, parameters);
            // process body
            methodStorage.AddBodyLine("pass");
        }

        private void ProcessOverloadingConstructor(IList<MethodData> overloadings, ClassStorage classStorage)
        {
            String destName = overloadings[0].DestName;
            String[] parameters = new[] {PythonSpecificDef.SelfArg, PythonSpecificDef.Args};
            MethodStorage methodStorage = classStorage.CreateMethodStorage(destName, parameters);
            foreach (MethodData overloading in overloadings)
                ProcessOverloadingCase(overloading, methodStorage);
            methodStorage.ImportStorage.AddImport("system");
            methodStorage.AddBodyLine("raise system.InvalidOperationException(\"Unrecognized combination of arguments\")");
        }

        private void ProcessMethods(IDictionary<String, IList<MethodData>> methods, ClassStorage classStorage)
        {
            foreach (String name in methods.Keys.Order())
            {
                IList<MethodData> overloads = methods[name];
                switch (overloads.Count)
                {
                    case 1:
                        ProcessNonOverloadingMethod(overloads[0], classStorage);
                        break;
                    case > 1:
                        ProcessOverloadingMethod(overloads, classStorage);
                        break;
                    default:
                        throw new InvalidOperationException($"Bad methods data for method named \"{name}\"");
                }
            }
        }

        private void ProcessNonOverloadingMethod(MethodData data, ClassStorage classStorage)
        {
            String[] parameters = data
                .Symbol
                .Parameters
                .Select(parameter => parameter.Name)
                .ToArray();
            if (!data.Symbol.IsStatic)
                parameters = new[] {PythonSpecificDef.SelfArg}.Concat(parameters).ToArray();
            MethodStorage methodStorage = classStorage.CreateMethodStorage(data.DestName, parameters);
            if (data.Symbol.IsStatic)
                methodStorage.AddDecorator(PythonSpecificDef.StaticMethod);
            if (data.Symbol.IsAbstract)
            {
                methodStorage.ImportStorage.AddImport("system");
                methodStorage.AddBodyLine("raise system.InvalidOperationException(\"Abstract method call\")");
            }
            else
            {
                // process body
                methodStorage.AddBodyLine("pass");
            }
        }

        private void ProcessOverloadingMethod(IList<MethodData> overloadings, ClassStorage classStorage)
        {
            String destName = overloadings[0].DestName;
            Boolean isStatic = overloadings[0].Symbol.IsStatic;
            String[] parameters = isStatic ? new[] {PythonSpecificDef.Args} : new[] {PythonSpecificDef.SelfArg, PythonSpecificDef.Args};
            MethodStorage methodStorage = classStorage.CreateMethodStorage(destName, parameters);
            if (isStatic)
                methodStorage.AddDecorator(PythonSpecificDef.StaticMethod);
            foreach (MethodData overloading in overloadings)
                ProcessOverloadingCase(overloading, methodStorage);
            methodStorage.ImportStorage.AddImport("system");
            methodStorage.AddBodyLine("raise system.InvalidOperationException(\"Unrecognized combination of arguments\")");
        }

        private void ProcessOverloadingCase(MethodData data, MethodStorage methodStorage)
        {
            String[] argumentTypeChecks = new String[data.Symbol.Parameters.Length + 1];
            argumentTypeChecks[0] = $"(len(args) == {data.Symbol.Parameters.Length})";
            for (Int32 index = 0; index < data.Symbol.Parameters.Length; ++index)
            {
                argumentTypeChecks[index + 1] = GenerateArgumentTypeCheck($"args[{index}]", data.Symbol.Parameters[index].Type, methodStorage);
            }
            String argumentTypeCheck = String.Join(" and ", argumentTypeChecks);
            methodStorage.AddBodyLine($"if {argumentTypeCheck}:");
            methodStorage.IncreaseLocalIndentation(StorageDef.IndentationDelta);
            if (data.Symbol.IsAbstract)
            {
                methodStorage.ImportStorage.AddImport("system");
                methodStorage.AddBodyLine("raise system.InvalidOperationException(\"Abstract method call\")");
            }
            else
            {
                // process body
                methodStorage.AddBodyLine("pass");
            }
            methodStorage.DecreaseLocalIndentation(StorageDef.IndentationDelta);
        }

        private String GenerateArgumentTypeCheck(String parameterName, ITypeSymbol parameterType, MethodStorage methodStorage)
        {
            if (parameterType.IsBoolean())
                return $"isinstance({parameterName}, bool)";
            if (parameterType.IsIntegerNumber())
                return $"isinstance({parameterName}, int)";
            if (parameterType.IsFloatNumber())
                return $"isinstance({parameterName}, float)";
            if (parameterType.IsString())
                return $"(isinstance({parameterName}, str) or {parameterName} is None)";
            String sourceTypeName = parameterType.Name;
            String sourceNamespaceName = parameterType.ContainingNamespace.ToString()!;
            String destTypeName = _appData.NameTransformer.TransformTypeName(sourceTypeName);
            String destModuleName = _appData.NameTransformer.TransformNamespaceName(sourceNamespaceName);
            methodStorage.ImportStorage.AddImport(destModuleName);
            return $"(isinstance({parameterName}, {destModuleName}.{destTypeName}) or {parameterName} is None)";
        }

        private readonly SemanticModel _model;
        private readonly FileStorage _currentFile;
        private readonly AppData _appData;
    }

    internal record MethodData(String DestName, BaseMethodDeclarationSyntax Declaration, IMethodSymbol Symbol)
    {
        public static MethodData Create(TypeDeclarationSyntax type, MethodDeclarationSyntax method, SemanticModel model, AppData appData)
        {
            String typeName = type.Identifier.Text;
            String sourceName = method.Identifier.Text;
            IMethodSymbol methodSymbol = model.GetDeclaredSymbol(method).Must();
            MemberModifier modifier = methodSymbol.DeclaredAccessibility.ToMemberModifier();
            switch (methodSymbol.MethodKind)
            {
                case MethodKind.Ordinary:
                    CheckMethod(methodSymbol);
                    String destName = appData.NameTransformer.TransformMethodName(typeName, sourceName, modifier);
                    return new MethodData(destName, method, methodSymbol);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported method named \"{sourceName}\"");
            }
        }

        public static MethodData Create(TypeDeclarationSyntax type, ConstructorDeclarationSyntax constructor, SemanticModel model, AppData appData)
        {
            IMethodSymbol methodSymbol = model.GetDeclaredSymbol(constructor).Must();
            MemberModifier modifier = methodSymbol.DeclaredAccessibility.ToMemberModifier();
            switch (methodSymbol.MethodKind)
            {
                case MethodKind.Constructor when modifier == MemberModifier.Public:
                    CheckMethod(methodSymbol);
                    return new MethodData(PythonSpecificDef.Constructor, constructor, methodSymbol);
                default:
                    throw new UnsupportedSyntaxException("Unsupported constructor");
            }
        }

        private static void CheckMethod(IMethodSymbol methodSymbol)
        {
            switch (methodSymbol)
            {
                case var _ when methodSymbol.IsAsync:
                    throw new UnsupportedSyntaxException($"Unsupported async method named \"{methodSymbol.Name}\"");
            }
        }
    }

    internal static class TypeDeclarationHelper
    {
        public static T[] GetMembers<T>(this TypeDeclarationSyntax type) where T : MemberDeclarationSyntax
        {
                return type.Members.OfType<T>().ToArray();
        }
    }

    internal static class TypeHelper
    {
        public static Boolean IsBoolean(this ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_Boolean => true,
                _ => false
            };
        }

        public static Boolean IsIntegerNumber(this ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_SByte => true,
                SpecialType.System_Byte => true,
                SpecialType.System_Int16 => true,
                SpecialType.System_UInt16 => true,
                SpecialType.System_Int32 => true,
                SpecialType.System_UInt32 => true,
                SpecialType.System_Int64 => true,
                SpecialType.System_UInt64 => true,
                _ => false
            };
        }

        public static Boolean IsFloatNumber(this ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_Single => true,
                SpecialType.System_Double => true,
                SpecialType.System_Decimal => true,
                _ => false
            };
        }

        public static Boolean IsString(this ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_String => true,
                _ => false
            };
        }
    }
}
