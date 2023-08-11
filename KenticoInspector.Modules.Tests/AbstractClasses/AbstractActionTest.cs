using KenticoInspector.Core;

using NUnit.Framework;

namespace KenticoInspector.Modules.Tests
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
            _mockDatabaseService.Invocations.Clear();
        }
    }
}
