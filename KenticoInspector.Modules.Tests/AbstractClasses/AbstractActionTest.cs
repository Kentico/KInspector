using KenticoInspector.Core;

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
    }
}
