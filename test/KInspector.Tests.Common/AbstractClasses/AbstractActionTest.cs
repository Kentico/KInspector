using KInspector.Core;

using NUnit.Framework;

namespace KInspector.Tests.Common
{
    public class AbstractActionTest<ModuleType, TermsType, OptionsType> : AbstractModuleTest<ModuleType, TermsType>
        where ModuleType : AbstractModule<TermsType>
        where TermsType : new()
        where OptionsType : new()
    {
        public AbstractActionTest(int majorVersion) : base(majorVersion)
        {
        }

        [SetUp]
        public void SetUp()
        {
            _mockDatabaseService?.Invocations.Clear();
        }
    }
}
