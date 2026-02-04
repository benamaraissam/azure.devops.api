using System;
using System.Net;
using System.Threading.Tasks;
using Azure.Devops.Api.Legacy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Azure.Devops.Api.Test.Legacy
{
    [TestClass]
    public class RestRequesterTest
    {
        private class TestRestRequester : RestRequester
        {
            protected override string Url => "https://test.example.com";

            public async Task<T> TestGet<T>(string endpoint)
            {
                return await Get<T>(endpoint);
            }

            public async Task<T> TestPut<T>(string endpoint, object obj)
            {
                return await Put<T>(endpoint, obj);
            }
        }

        [TestMethod]
        public async Task ExecuteWithRetry_SucceedsOnSecondAttempt()
        {
            // Arrange
            var mockClient = new Mock<RestClient>();
            var requester = new TestRestRequester();

            // Set up the client to return different responses on each call
            // First call: 503 Service Unavailable (transient)
            // Second call: 200 OK (success)

            var firstResponse = new RestResponse<TestClass>
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                StatusDescription = "Service Unavailable",
                Content = "",
                ErrorException = null
            };

            var secondResponse = new RestResponse<TestClass>
            {
                StatusCode = HttpStatusCode.OK,
                StatusDescription = "OK",
                Content = "{\"Name\":\"Test\"}",
                ErrorException = null
            };

            mockClient.Setup(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()))
                .ReturnsAsync(firstResponse)
                .ReturnsAsync(secondResponse);

            // Replace the client with our mock
            var field = typeof(RestRequester).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(requester, mockClient.Object);

            // Act
            var result = await requester.TestGet<TestClass>("/test");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Name);
            mockClient.Verify(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ExecuteWithRetry_FailsAfterMaxRetries()
        {
            // Arrange
            var mockClient = new Mock<RestClient>();
            var requester = new TestRestRequester();

            // Set up the client to always return 503 Service Unavailable
            var failureResponse = new RestResponse<TestClass>
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                StatusDescription = "Service Unavailable",
                Content = "",
                ErrorException = null
            };

            mockClient.Setup(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()))
                .ReturnsAsync(failureResponse)
                .ReturnsAsync(failureResponse)
                .ReturnsAsync(failureResponse);

            // Replace the client with our mock
            var field = typeof(RestRequester).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(requester, mockClient.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => requester.TestGet<TestClass>("/test"));
            mockClient.Verify(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task ExecuteWithRetry_DoesNotRetryOnNonTransientErrors()
        {
            // Arrange
            var mockClient = new Mock<RestClient>();
            var requester = new TestRestRequester();

            // Set up the client to return 404 Not Found (non-transient)
            var notFoundResponse = new RestResponse<TestClass>
            {
                StatusCode = HttpStatusCode.NotFound,
                StatusDescription = "Not Found",
                Content = "",
                ErrorException = null
            };

            mockClient.Setup(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()))
                .ReturnsAsync(notFoundResponse);

            // Replace the client with our mock
            var field = typeof(RestRequester).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(requester, mockClient.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => requester.TestGet<TestClass>("/test"));
            mockClient.Verify(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteWithRetry_RetryOnNetworkException()
        {
            // Arrange
            var mockClient = new Mock<RestClient>();
            var requester = new TestRestRequester();

            // First call: network exception (transient)
            // Second call: success
            var networkException = new HttpRequestException("Network error");
            var successResponse = new RestResponse<TestClass>
            {
                StatusCode = HttpStatusCode.OK,
                StatusDescription = "OK",
                Content = "{\"Name\":\"Test\"}",
                ErrorException = null
            };

            mockClient.Setup(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()))
                .ThrowsAsync(networkException)
                .ReturnsAsync(successResponse);

            // Replace the client with our mock
            var field = typeof(RestRequester).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(requester, mockClient.Object);

            // Act
            var result = await requester.TestGet<TestClass>("/test");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Name);
            mockClient.Verify(c => c.ExecuteAsync<TestClass>(It.IsAny<RestRequest>()), Times.Exactly(2));
        }

        private class TestClass
        {
            public string Name { get; set; }
        }
    }
}
