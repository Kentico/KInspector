namespace KenticoInspector.Core.Models
{
    /// <summary>
    /// Represents the text between tokens.
    /// </summary>
    internal class EmptyToken : Token
    {
        internal EmptyToken(string tokenExpression) : base(tokenExpression)
        {
        }
    }
}