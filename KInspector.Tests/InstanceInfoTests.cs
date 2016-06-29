using Kentico.KInspector.Core;
using NUnit.Framework;

namespace Kentico.KInspector.Tests
{
    [TestFixture]
    public class InstanceInfoTests
    {
        [TestCase("http://relative.url", "http://relative.url/")]
        [TestCase("http://relative.url/", "http://relative.url/")]
        [TestCase("http://relative.url/nobackslash", "http://relative.url/nobackslash/")]
        [TestCase("http://relative.url/backslash/", "http://relative.url/backslash/")]
        public void EnsureTrailingSlash(string url, string expected)
        {
            var instanceConfig = new InstanceConfig
            {
                Url = url
            };
            
            var instanceInfo = new InstanceInfo(instanceConfig);

            Assert.AreEqual(expected, instanceInfo.Uri.AbsoluteUri);
        }
    }
}
