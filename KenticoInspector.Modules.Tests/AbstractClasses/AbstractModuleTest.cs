using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Core.Tokens;
using KenticoInspector.Modules.Tests.Helpers;

using Moq;

namespace KenticoInspector.Modules.Tests
{
    public abstract class AbstractModuleTest<ModuleType, TermsType>
        where ModuleType : AbstractModule<TermsType>
        where TermsType : new()
    {
        protected Instance _mockInstance;
        protected InstanceDetails _mockInstanceDetails;
        protected Mock<IDatabaseService> _mockDatabaseService;
        protected Mock<IInstanceService> _mockInstanceService;
        protected Mock<IModuleMetadataService> _mockModuleMetadataService;
        protected Mock<ICmsFileService> _mockCmsFileService;

        protected AbstractModuleTest(int majorVersion)
        {
            var reportCodename = AbstractModule<TermsType>.GetCodename(typeof(ModuleType));

            TokenExpressionResolver.RegisterTokenExpressions(typeof(TokenExpressionResolver).Assembly);

            InitializeCommonMocks(majorVersion, reportCodename);
        }

        protected virtual void InitializeCommonMocks(int majorVersion, string moduleCodename)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
            _mockCmsFileService = MockCmsFileServiceHelper.SetupMockCmsFileService();
            _mockModuleMetadataService = MockModuleMetadataServiceHelper.GetBasicModuleMetadataService<TermsType>(moduleCodename);
        }
    }
}