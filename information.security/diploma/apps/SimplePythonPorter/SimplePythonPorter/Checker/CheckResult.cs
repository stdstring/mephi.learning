using SimplePythonPorter.Common;

namespace SimplePythonPorter.Checker
{
    internal abstract record CheckResult
    {
        internal record Ok : CheckResult;

        internal record Error(String Reason) : CheckResult;
    }

    internal static class CheckResultExtensions
    {
        public static void MustSuccess(this CheckResult result, String errorTemplate = "")
        {
            switch (result)
            {
                case CheckResult.Error(Reason: var reason):
                    throw new UnsupportedSyntaxException(String.IsNullOrEmpty(errorTemplate) ? reason : String.Format(errorTemplate, reason));
            }
        }
    }
}
