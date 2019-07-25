using System.Collections.Generic;

namespace KenticoInspector.Core.Tokens
{
    internal interface IToken
    {
        string FillFrom(string rawToken, IDictionary<string, object> valuesDictionary);
    }
}