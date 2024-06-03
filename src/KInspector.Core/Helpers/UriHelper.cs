namespace KInspector.Core.Helpers
{
    public static class UriHelper
    {
        public static Uri CombineUrl(string baseUrl, string relativePath)
        {
            return new Uri($"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}");
        }
    }
}