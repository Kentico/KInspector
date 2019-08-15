using System;

namespace KenticoInspector.Core.Extensions
{
    public static class StringExtensions
    {
        public static (string first, string second) SplitAtFirst(this string source, char splitChar)
        {
            var index = source.IndexOf(splitChar);

            if (index < 0)
            {
                return (source, null);
            }

            var charSpan = source.AsSpan();

            return (new string(charSpan.Slice(0, index)), new string(charSpan.Slice(index + 1)));
        }
    }
}