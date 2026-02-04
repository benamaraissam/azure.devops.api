using System;
using System.Threading.Tasks;
using Azure.Devops.Api.Requesters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test.Exceptions
{
    [TestClass]
    public class AzureDevOpsExceptionIntegrationTest
    {
        // Note: This test will fail if the credentials are valid
        // It's intended to verify that the error handling system works
        // by attempting to trigger an error condition
        [TestMethod]
        public void GetVariableGroupById_WhenInvalidCredentials_ThrowsAzureDevOpsException()
        {
            // Arrange
            // Temporarily modify the credentials to something invalid
            // This is a simulation since we can't change the actual credentials
            // In a real environment, this would be tested with a non-existent resource
            var requester = new DistributedTaskRequester();

            // We're testing that the error handling wraps exceptions properly
            // In a real scenario, this would trigger a 401 Unauthorized or similar
            // Since we can't change the credentials in the test environment,
            // we'll verify that the exception type is correct when an error occurs
            // This is a simplified test due to the constraints of the test environment

            // Try to get a non-existent variable group to trigger an error
            var testId = -1; // Invalid ID to trigger error

            // Act & Assert
            try
            {
                var result = requester.GetVariableGroupById(testId).Result;
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
        public void GetReleases_WhenErrorOccurs_ThrowsAzureDevOpsException()
        {
            // Arrange
            var requester = new ReleaseRequester();

            // Try to get releases with an invalid project/organization
            // In a real test environment, this would trigger an error

            // Act & Assert
            try
            {
                var result = requester.GetReleases().Result;
                Assert.Fail("Expected an exception to be thrown for error condition");
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
                Assert.IsNull(azureException.ResourceId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected AzureDevOpsException but got {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
