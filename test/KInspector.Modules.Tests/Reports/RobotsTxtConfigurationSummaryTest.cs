﻿using KInspector.Core.Constants;
using KInspector.Tests.Common.Helpers;
using KInspector.Reports.RobotsTxtConfigurationSummary;
using KInspector.Reports.RobotsTxtConfigurationSummary.Models;

using Moq;
using Moq.Protected;

using NUnit.Framework;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class RobotsTxtConfigurationSummaryTest : AbstractModuleTest<Report, Terms>
    {
        private Report? _mockReport;

        public RobotsTxtConfigurationSummaryTest(int majorVersion) : base(majorVersion)
        {
        }

        [Test]
        public async Task Should_ReturnGoodStatus_WhenRobotsTxtFound()
        {
            // Arrange
            _mockReport = ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = _mockConfigService.Object.GetCurrentInstance();

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Good);

            var baseUri = new Uri(mockInstance?.AdministrationUrl ?? string.Empty);
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
        public async Task Should_ReturnGoodStatus_WhenSiteIsInSubDirectoryAndRobotsTxtFound()
        {
            // Arrange
            _mockReport = ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = _mockConfigService.Object.GetCurrentInstance();

            var baseUrl = mockInstance?.AdministrationUrl;
            mockInstance.AdministrationUrl += "/subdirectory";

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Good);

            var expectedUri = new Uri($"{baseUrl}/{Constants.RobotsTxtRelativePath}");

            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        [Test]
        public async Task Should_ReturnWarningStatus_WhenRobotsTxtNotFound()
        {
            // Arrange
            _mockReport = ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode.NotFound, out Mock<HttpMessageHandler> mockHttpMessageHandler);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        private Report ConfigureReportAndHandlerWithHttpClientReturning(HttpStatusCode httpStatusCode, out Mock<HttpMessageHandler> mockHttpMessageHandler)
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
            var report = new Report(_mockConfigService.Object, _mockModuleMetadataService.Object, httpClient);
            MockModuleMetadataServiceHelper.SetupModuleMetadataService<Terms>(_mockModuleMetadataService, report);

            return report;
        }
    }
}