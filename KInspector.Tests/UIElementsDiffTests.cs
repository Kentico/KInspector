using Kentico.KInspector.Modules;
using NUnit.Framework;

namespace Kentico.KInspector.Tests
{
    [TestFixture]
    public class UIElementsDiffTests
    {
        [TestCase("<helptopicname>{% EditedObject.IssueIsABTest|(user)administrator|(hash)12572368016007acd9b60a90ba5bae7a67c8072a68ff693f2fff3e4b95071765%}</helptopicname>",
                  "<helptopicname>{% EditedObject.IssueIsABTest|(user)administrator%}</helptopicname>")]
        public void RemoveHash(string input, string expected)
        {
            string result = UIElementsDiff.RemoveHash(input);

            Assert.AreEqual(expected, result);
        }
    }
}
