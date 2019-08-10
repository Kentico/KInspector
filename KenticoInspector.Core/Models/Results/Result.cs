namespace KenticoInspector.Core.Models.Results
{
    public class Result
    {
        public string Name { get; set; }

        public static implicit operator Result(string stringData) => new StringResult(stringData);

        public static implicit operator Result(Term term) => new StringResult(term);

        public static implicit operator string(Result result) => (result as StringResult)?.String;
    }
}