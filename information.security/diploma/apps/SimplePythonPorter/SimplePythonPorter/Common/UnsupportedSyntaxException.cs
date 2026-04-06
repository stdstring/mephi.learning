namespace SimplePythonPorter.Common
{
    internal class UnsupportedSyntaxException : Exception
    {
        public UnsupportedSyntaxException(string reason) : base(reason)
        {
        }

        public UnsupportedSyntaxException(string reason, Exception innerException) : base(reason, innerException)
        {
        }
    }
}
