namespace KenticoInspector.Core.Models.Results
{
    public abstract class Result
    {
        public string Label { get; protected set; }

        public abstract bool HasData { get; }

        public static implicit operator Result(Term term) => new StringResult(term);
    }
}