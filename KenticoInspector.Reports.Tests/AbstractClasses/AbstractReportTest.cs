using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Core.Tokens;
using KenticoInspector.Reports.Tests.Helpers;

using Moq;

namespace KenticoInspector.Reports.Tests
{
    public abstract class AbstractReportTest<ReportType, TermsType>
        where ReportType : AbstractReport<TermsType>
        where TermsType : new()
    {
        protected Instance _mockInstance;
        protected InstanceDetails _mockInstanceDetails;
        protected Mock<IDatabaseService> _mockDatabaseService;
        protected Mock<IInstanceService> _mockInstanceService;
        protected Mock<IReportMetadataService> _mockReportMetadataService;
        protected Mock<ICmsFileService> _mockCmsFileService;

        public AbstractReportTest(int majorVersion)
        {
            var reportCodename = AbstractReport<TermsType>.GetCodename(typeof(ReportType));

            TokenProcessor.RegisterTokens(typeof(TokenProcessor).Assembly);

            InitializeCommonMocks(majorVersion, reportCodename);
        }

        protected virtual void InitializeCommonMocks(int majorVersion, string reportCodename)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
            _mockCmsFileService = MockCmsFileServiceHelper.SetupMockCmsFileService();
            _mockReportMetadataService = MockReportMetadataServiceHelper.GetBasicReportMetadataService<TermsType>(reportCodename);
        }
    }
}