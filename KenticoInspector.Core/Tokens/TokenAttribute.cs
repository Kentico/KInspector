using System;

namespace KenticoInspector.Core.Tokens
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TokenAttribute : Attribute
    {
        internal string Pattern { get; }

        public TokenAttribute(string pattern)
        {
            Pattern = pattern;
        }

    }
}