namespace KInspector.Core.Tokens
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TokenExpressionAttribute : Attribute
    {
        internal string Pattern { get; }

        public TokenExpressionAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}