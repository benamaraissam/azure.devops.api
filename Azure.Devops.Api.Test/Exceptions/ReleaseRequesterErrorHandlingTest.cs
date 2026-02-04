using System;
using System.Threading.Tasks;
using Azure.Devops.Api.Requesters;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test.Exceptions
{
    [TestClass]
    public class ReleaseRequesterErrorHandlingTest
    {
        [TestMethod]
        public void GetReleaseById_WhenErrorOccurs_ThrowsAzureDevOpsException()
        {
            // Arrange
            var requester = new ReleaseRequester();

            // Try to get a non-existent release to trigger an error
            var testId = -1; // Invalid ID to trigger error

            // Act & Assert
            try
            {
                var result = requester.GetReleaseById(testId).Result;
                Assert.Fail("Expected an exception to be thrown for invalid ID");
            }
            catch (AggregateException ae)
            {
                // Unwrap the AggregateException
                var innerException = ae.InnerException;
                Assert.IsInstanceOfType(innerException, typeof(AzureDevops.Api.Exceptions.AzureDevOpsException));

                var azureException = (AzureDevops.Api.Exceptions.AzureDevOpsException)innerException;

                // Verify the custom exception has the required properties
                Assert.IsTrue(azureException.HttpStatusCode >= 400,
                    $"Expected HTTP status code >= 400, got {azureException.HttpStatusCode}");
                Assert.IsFalse(string.IsNullOrEmpty(azureException.ErrorCode));
                Assert.IsFalse(string.IsNullOrEmpty(azureException.Endpoint));
                Assert.AreEqual("benamaraissam", azureException.Organization);
                Assert.AreEqual("TuniTv", azureException.Project);
                Assert.AreEqual("-1", azureException.ResourceId);
            }
            catch (Exception ex)
            {
                // If we get here, it means our error handling didn't work as expected
                Assert.Fail($"Expected AzureDevOpsException but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        [TestMethod]
        public void UpdateRelease_WhenErrorOccurs_ThrowsAzureDevOpsException()
        {
            // Arrange
            var requester = new ReleaseRequester();

            // Try to update a release with an invalid ID
            var testId = -1; // Invalid ID to trigger error
            var release = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release(); // Create a minimal release object

            // Act & Assert
            try
            {
                var result = requester.UpdateRelease(testId, release).Result;
                Assert.Fail("Expected an exception to be thrown for invalid ID");
            }
            catch (AggregateException ae)
            {
                var innerException = ae.InnerException;
                Assert.IsInstanceOfType(innerException, typeof(AzureDevops.Api.Exceptions.AzureDevOpsException));

                var azureException = (AzureDevops.Api.Exceptions.AzureDevOpsException)innerException;

                // Verify the custom exception has the required properties
                Assert.IsTrue(azureException.HttpStatusCode >= 400,
                    $"Expected HTTP status code >= 400, got {azureException.HttpStatusCode}");
                Assert.IsFalse(string.IsNullOrEmpty(azureException.ErrorCode));
                Assert.IsFalse(string.IsNullOrEmpty(azureException.Endpoint));
                Assert.AreEqual("benamaraissam", azureException.Organization);
                Assert.AreEqual("TuniTv", azureException.Project);
                Assert.AreEqual("-1", azureException.ResourceId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected AzureDevOpsException but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        [TestMethod]
        public void DeleteRelease_WhenErrorOccurs_ThrowsAzureDevOpsException()
        {
            // Arrange
            var requester = new ReleaseRequester();

            // Try to delete a non-existent release
            var testId = -1; // Invalid ID to trigger error

            // Act & Assert
            try
            {
                requester.DeleteRelease(testId).Wait();
                Assert.Fail("Expected an exception to be thrown for invalid ID");
            }
            catch (AggregateException ae)
            {
                var innerException = ae.InnerException;
                Assert.IsInstanceOfType(innerException, typeof(AzureDevops.Api.Exceptions.AzureDevOpsException));

                var azureException = (AzureDevops.Api.Exceptions.AzureDevOpsException)innerException;

                // Verify the custom exception has the required properties
                Assert.IsTrue(azureException.HttpStatusCode >= 400,
                    $"Expected HTTP status code >= 400, got {azureException.HttpStatusCode}");
                Assert.IsFalse(string.IsNullOrEmpty(azureException.ErrorCode));
                Assert.IsFalse(string.IsNullOrEmpty(azureException.Endpoint));
                Assert.AreEqual("benamaraissam", azureException.Organization);
                Assert.AreEqual("TuniTv", azureException.Project);
                Assert.AreEqual("-1", azureException.ResourceId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected AzureDevOpsException but got {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
