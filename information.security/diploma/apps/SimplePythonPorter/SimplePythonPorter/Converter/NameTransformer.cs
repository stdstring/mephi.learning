namespace SimplePythonPorter.Converter
{
    internal class NameTransformer
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

        public String TransformMethodName(String typeName, String methodName)
        {
            return methodName;
        }

        public String TransformPropertyName(String typeName, String propertyName)
        {
            return propertyName;
        }

        public String TransformFieldName(String typeName, String fieldName)
        {
            return fieldName;
        }

        public String TransformStaticReadonlyFieldName(String typeName, String fieldName)
        {
            return fieldName;
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
