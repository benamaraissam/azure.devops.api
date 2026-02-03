using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.Devops.Api.Test
{
    /// <summary>
    /// Manages Azure DevOps credentials from environment variables.
    /// This class provides a secure way to handle credentials without hardcoding them.
    /// Environment variables required:
    /// - AZURE_DEVOPS_TOKEN: Personal access token for authentication
    /// - AZURE_DEVOPS_ORGANIZATION: Azure DevOps organization name
    /// - AZURE_DEVOPS_PROJECT: Azure DevOps project name
    /// - AZURE_DEVOPS_TEST_PROJECT: Test project name (optional, defaults to AZURE_DEVOPS_PROJECT)
    /// </summary>
    public static class TestCredentials
    {
        private const string TokenEnvVar = "AZURE_DEVOPS_TOKEN";
        private const string OrganizationEnvVar = "AZURE_DEVOPS_ORGANIZATION";
        private const string ProjectEnvVar = "AZURE_DEVOPS_PROJECT";
        private const string TestProjectEnvVar = "AZURE_DEVOPS_TEST_PROJECT";

        /// <summary>
        /// Gets the personal access token from environment variables.
        /// Throws exception if not found.
        /// </summary>
        public static string TOKEN => GetRequiredEnvironmentVariable(TokenEnvVar);

        /// <summary>
        /// Gets the Azure DevOps organization from environment variables.
        /// Throws exception if not found.
        /// </summary>
        public static string ORGANIZATION => GetRequiredEnvironmentVariable(OrganizationEnvVar);

        /// <summary>
        /// Gets the Azure DevOps project from environment variables.
        /// Throws exception if not found.
        /// </summary>
        public static string PROJECT => GetRequiredEnvironmentVariable(ProjectEnvVar);

        /// <summary>
        /// Gets the test project name from environment variables.
        /// If not specified, defaults to the main project.
        /// </summary>
        public static string TEST_PROJECT =>
            Environment.GetEnvironmentVariable(TestProjectEnvVar) ?? PROJECT;

        private static string GetRequiredEnvironmentVariable(string variableName)
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(
                    $"Required environment variable '{variableName}' is not set. " +
                    "Please set it to your Azure DevOps credentials.");
            }
            return value;
        }

        /// <summary>
        /// Validates that all required environment variables are set.
        /// </summary>
        public static void ValidateEnvironmentVariables()
        {
            try
            {
                // This will throw if any required variable is missing
                var token = TOKEN;
                var organization = ORGANIZATION;
                var project = PROJECT;

                // Test project is optional
                _ = TEST_PROJECT;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    "Failed to validate Azure DevOps integration test configuration. " +
                    "Please ensure all required environment variables are set:\n" +
                    $"- {TokenEnvVar}: Personal access token\n" +
                    $"- {OrganizationEnvVar}: Azure DevOps organization name\n" +
                    $"- {ProjectEnvVar}: Azure DevOps project name\n" +
                    $"- {TestProjectEnvVar} (optional): Test project name (defaults to {ProjectEnvVar})",
                    ex);
            }
        }
    }
}
```

Now I'll create the base integration test class with resource cleanup functionality:
