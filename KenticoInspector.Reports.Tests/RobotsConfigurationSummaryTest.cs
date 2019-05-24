using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Reports.RobotsConfigurationSummary;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class RobotsConfigurationSummaryTest
    {
        private Mock<IDatabaseService> _mockDatabaseService;
        private Instance _mockInstance;
        private InstanceDetails _mockInstanceDetails;
        private Mock<IInstanceService> _mockInstanceService;
        private Report _mockReport;

        public RobotsConfigurationSummaryTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
        }

        [Test]
        public void Should_ReturnGoodStatus_WhenRobotsTxtFound()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK })
                .Verifiable();

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, httpClient);

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);

            var baseUri = new Uri(_mockInstance.Url);
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
            var baseUrl = _mockInstance.Url;
            _mockInstance.Url += "/subdirectory";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, httpClient);

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);
            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);

            var expectedUri = new Uri($"{baseUrl}/{Constants.RobotsTxtRelativePath}");
            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        [Test]
        public void Should_ReturnWarningStatus_WhenRobotsTxtNotFound()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, httpClient);
            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);
            // Assert
            Assert.That(results.Status == ReportResultsStatus.Warning);
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
        }
    }
}