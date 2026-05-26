using Microsoft.CodeAnalysis;
using SimplePythonPorter.Common;

namespace SimplePythonPorter.Converter
{
    public enum MemberModifier
    {
        Public,
        Protected,
        Private
    }

    internal static class AccessibilityHelper
    {
        public static MemberModifier ToMemberModifier(this Accessibility value)
        {
            return value switch
            {
                Accessibility.Public => MemberModifier.Public,
                Accessibility.Protected => MemberModifier.Protected,
                Accessibility.Private => MemberModifier.Private,
                _ => throw new UnsupportedSyntaxException($"Unsupported accessibility value: {value}")
            };
        }
    }

    public class NameTransformer
    {
        public String TransformFileObjectName(String fileObjectName)
        {
            return fileObjectName;
        }

        public String TransformNamespaceName(String namespaceName)
        {
            return namespaceName.ToLower();
        }

        public String TransformTypeName(String typeName)
        {
            return typeName;
        }

        public String TransformMethodName(String typeName, String methodName, MemberModifier methodModifier)
        {
            return methodModifier switch
            {
                MemberModifier.Public => methodName,
                MemberModifier.Protected => $"_{methodName}",
                MemberModifier.Private => $"__{methodName}",
                _ => throw new InvalidOperationException("Unexpected value of method's modifier")
            };
        }

        public String TransformPropertyName(String typeName, String propertyName, MemberModifier propertyModifier)
        {
            return propertyModifier switch
            {
                MemberModifier.Public => propertyName,
                MemberModifier.Protected => $"_{propertyName}",
                MemberModifier.Private => $"__{propertyName}",
                _ => throw new InvalidOperationException("Unexpected value of property's modifier")
            };
        }

        public String TransformFieldName(String typeName, String fieldName, MemberModifier fieldModifier)
        {
            return fieldModifier switch
            {
                MemberModifier.Public => fieldName,
                MemberModifier.Protected => $"_{fieldName}",
                MemberModifier.Private => $"__{fieldName}",
                _ => throw new InvalidOperationException("Unexpected value of field's modifier")
            };
        }

        public String TransformEnumValueName(String typeName, String enumValueName)
        {
            return enumValueName;
        }

        public String TransformLocalVariableName(String variableName)
        {
            return variableName;
        }
    }
}
