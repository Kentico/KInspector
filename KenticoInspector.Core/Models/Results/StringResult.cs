namespace KenticoInspector.Core.Models.Results
{
    public class StringResult : Result
    {
        public string String { get; set; }

        internal StringResult(string stringData) => String = stringData;
    }
}