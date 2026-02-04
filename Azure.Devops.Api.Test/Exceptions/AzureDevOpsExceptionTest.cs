using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.Devops.Api.Exceptions;

namespace Azure.Devops.Api.Test.Exceptions
{
    [TestClass]
    public class AzureDevOpsExceptionTest
    {
        [TestMethod]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            // Arrange
            string message = "Test error message";
            int httpStatusCode = 404;
            string errorCode = "NotFound";
            string endpoint = "https://dev.azure.com/org/project/_apis/resource";
            string organization = "org";
            string project = "project";
            string resourceId = "123";

            // Act
            var exception = new AzureDevOpsException(
                message,
                httpStatusCode,
                errorCode,
                endpoint,
                organization,
                project,
                resourceId);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(httpStatusCode, exception.HttpStatusCode);
            Assert.AreEqual(errorCode, exception.ErrorCode);
            Assert.AreEqual(endpoint, exception.Endpoint);
            Assert.AreEqual(organization, exception.Organization);
            Assert.AreEqual(project, exception.Project);
            Assert.AreEqual(resourceId, exception.ResourceId);
        }

        [TestMethod]
        public void ToString_MethodIncludesAllDetails()
        {
            // Arrange
            var exception = new AzureDevOpsException(
                "Test error",
                500,
                "InternalServerError",
                "https://dev.azure.com/org/project/_apis/resource",
                "org",
                "project",
                "456");

            // Act
            string result = exception.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Test error"));
            Assert.IsTrue(result.Contains("HTTP Status: 500"));
            Assert.IsTrue(result.Contains("Error Code: InternalServerError"));
            Assert.IsTrue(result.Contains("Endpoint: https://dev.azure.com/org/project/_apis/resource"));
            Assert.IsTrue(result.Contains("Organization: org"));
            Assert.IsTrue(result.Contains("Project: project"));
            Assert.IsTrue(result.Contains("Resource ID: 456"));
        }

        [TestMethod]
        public void ToString_MethodOmitsResourceIdWhenNull()
        {
            // Arrange
            var exception = new AzureDevOpsException(
                "Test error",
                404,
                "NotFound",
                "https://dev.azure.com/org/project/_apis/resource",
                "org",
                "project");

            // Act
            string result = exception.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Test error"));
            Assert.IsTrue(result.Contains("HTTP Status: 404"));
            Assert.IsTrue(result.Contains("Error Code: NotFound"));
            Assert.IsTrue(result.Contains("Endpoint: https://dev.azure.com/org/project/_apis/resource"));
            Assert.IsTrue(result.Contains("Organization: org"));
            Assert.IsTrue(result.Contains("Project: project"));
            Assert.IsFalse(result.Contains("Resource ID:"));
        }
    }
}
