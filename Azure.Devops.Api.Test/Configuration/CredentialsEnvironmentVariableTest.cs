using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test.Configuration
{
    [TestClass]
    public class CredentialsEnvironmentVariableTest
    {
        [TestInitialize]
        public void Setup()
        {
            // Clear any existing environment variables that might interfere with tests
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_TOKEN", null);
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_PROJECT", null);
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_ORGANIZATION", null);
        }

        [TestMethod]
        public void TestDefaultValuesWhenEnvironmentVariablesNotSet()
        {
            // When environment variables are not set, should use defaults
            Assert.AreEqual("ycz4af65xdzmsgcs2tdgmroiaj36x5ao4ofqrntyvsesmavnqqfq", Azure.Devops.Api.Credentials.TOKEN);
            Assert.AreEqual("TuniTv", Azure.Devops.Api.Credentials.PROJECT);
            Assert.AreEqual("benamaraissam", Azure.Devops.Api.Credentials.ORGANIZATION);
        }

        [TestMethod]
        public void TestEnvironmentVariableValues()
        {
            // Set environment variables
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_TOKEN", "test-token");
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_PROJECT", "test-project");
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_ORGANIZATION", "test-organization");

            // Verify values are read from environment variables
            Assert.AreEqual("test-token", Azure.Devops.Api.Credentials.TOKEN);
            Assert.AreEqual("test-project", Azure.Devops.Api.Credentials.PROJECT);
            Assert.AreEqual("test-organization", Azure.Devops.Api.Credentials.ORGANIZATION);
        }

        [TestMethod]
        public void TestEmptyEnvironmentVariableUsesDefault()
        {
            // Set environment variable to empty string
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_TOKEN", "");
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_PROJECT", "");
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_ORGANIZATION", "");

            // Should use defaults when environment variables are empty
            Assert.AreEqual("ycz4af65xdzmsgcs2tdgmroiaj36x5ao4ofqrntyvsesmavnqqfq", Azure.Devops.Api.Credentials.TOKEN);
            Assert.AreEqual("TuniTv", Azure.Devops.Api.Credentials.PROJECT);
            Assert.AreEqual("benamaraissam", Azure.Devops.Api.Credentials.ORGANIZATION);
        }

        [TestMethod]
        public void TestNullEnvironmentVariableUsesDefault()
        {
            // Set environment variable to null (already cleared in TestInitialize)
            // Should use defaults when environment variables are null
            Assert.AreEqual("ycz4af65xdzmsgcs2tdgmroiaj36x5ao4ofqrntyvsesmavnqqfq", Azure.Devops.Api.Credentials.TOKEN);
            Assert.AreEqual("TuniTv", Azure.Devops.Api.Credentials.PROJECT);
            Assert.AreEqual("benamaraissam", Azure.Devops.Api.Credentials.ORGANIZATION);
        }
    }
}
