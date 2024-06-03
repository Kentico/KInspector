namespace KInspector.Core.Tokens
{
    internal interface ITokenExpression
    {
        string? Resolve(string tokenExpression, IDictionary<string, object?> tokenDictionary);
    }
}
