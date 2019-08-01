using System.Linq;

namespace KenticoInspector.Core.Extensions
{
    public static class StringExtensions
    {
        public static (string first, string second) SplitAtFirst(this string source, char splitChar)
        {
            var index = source.IndexOf(splitChar);

            var splitString = source.Split(splitChar);

            switch (splitString.Length)
            {
                case 1:
                    return (source, null);

                default:
                    return (source.Substring(0, index), source.Substring(index + 1, source.Length - index - 1));
            }
        }
    }
}