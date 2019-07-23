using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.RobotsConfigurationSummary;
using KenticoInspector.Reports.RobotsConfigurationSummary.Models;
using KenticoInspector.Reports.Tests.Helpers;

using Moq;
using Moq.Protected;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class RobotsConfigurationSummaryTest
    {
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IInstanceService> _mockInstanceService;
        private Mock<IReportMetadataService> _mockReportMetadataService;
        private RobotsConfigurationSummaryReport _mockReport;

        public RobotsConfigurationSummaryTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);

            _mockReportMetadataService = MockReportMetadataServiceHelper.GetReportMetadataService();
        }

        [Test]
        public void Should_ReturnGoodStatus_WhenRobotsTxtFound()
        {
            // Arrange
            _mockReport = ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = _mockInstanceService.Object.CurrentInstance;

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);

            var baseUri = new Uri(mockInstance.Url);
            var expectedUri = new Uri(baseUri, Constants.RobotsTxtRelativePath);

            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        private static void AssertUrlCalled(Mock<HttpMessageHandler> handlerMock, Uri expectedUri)
        {
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void Should_ReturnGoodStatus_WhenSiteIsInSubDirectoryAndRobotsTxtFound()
        {
            // Arrange

            _mockReport = ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = _mockInstanceService.Object.CurrentInstance;

            var baseUrl = mockInstance.Url;
            mockInstance.Url += "/subdirectory";

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);

            var expectedUri = new Uri($"{baseUrl}/{Constants.RobotsTxtRelativePath}");

            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        [Test]
        public void Should_ReturnWarningStatus_WhenRobotsTxtNotFound()
        {
            // Arrange
            _mockReport = ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode.NotFound, out Mock<HttpMessageHandler> mockHttpMessageHandler);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Warning);
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            var mockInstance = MockInstances.Get(majorVersion);

            var mockInstanceDetails = MockInstanceDetails.Get(majorVersion, mockInstance);

            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(mockInstance, mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(mockInstance);
        }

        private RobotsConfigurationSummaryReport ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode httpStatusCode, out Mock<HttpMessageHandler> mockHttpMessageHandler)
        {
            mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = httpStatusCode })
                .Verifiable();

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            var report = new RobotsConfigurationSummaryReport(_mockDatabaseService.Object, _mockInstanceService.Object, _mockReportMetadataService.Object, httpClient);

            MockReportMetadataServiceHelper.SetupReportMetadataService<Labels>(_mockReportMetadataService, report);

            return report;
        }
    }
}