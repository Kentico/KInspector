using KInspector.Core;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Core.Tokens;
using KInspector.Tests.Common.Helpers;

using Moq;

namespace KInspector.Tests.Common
{
    public abstract class AbstractModuleTest<ModuleType, TermsType>
        where ModuleType : AbstractModule<TermsType>
        where TermsType : new()
    {
        protected Instance? _mockInstance;
        protected InstanceDetails _mockInstanceDetails;
        protected Mock<IDatabaseService> _mockDatabaseService;
        protected Mock<IInstanceService> _mockInstanceService;
        protected Mock<IModuleMetadataService> _mockModuleMetadataService;
        protected Mock<IConfigService> _mockConfigService;
        protected Mock<ICmsFileService> _mockCmsFileService;

        protected AbstractModuleTest(int majorVersion)
        {
            var moduleCodename = AbstractModule<TermsType>.GetCodename(typeof(ModuleType));

            TokenExpressionResolver.RegisterTokenExpressions(typeof(TokenExpressionResolver).Assembly);

            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
            _mockConfigService = MockConfigServiceHelper.SetupMockConfigService(_mockInstance);
            _mockCmsFileService = MockCmsFileServiceHelper.SetupMockCmsFileService();
            _mockModuleMetadataService = MockModuleMetadataServiceHelper.GetBasicModuleMetadataService<TermsType>(moduleCodename);
        }
    }
}