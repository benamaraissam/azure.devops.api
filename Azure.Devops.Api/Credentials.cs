using System;
using System.Diagnostics;

namespace Azure.Devops.Api
{
    public static class Credentials
    {
        private static readonly string _token = GetEnvironmentVariable("AZURE_DEVOPS_TOKEN", "ycz4af65xdzmsgcs2tdgmroiaj36x5ao4ofqrntyvsesmavnqqfq");
        private static readonly string _project = GetEnvironmentVariable("AZURE_DEVOPS_PROJECT", "TuniTv");
        private static readonly string _organization = GetEnvironmentVariable("AZURE_DEVOPS_ORGANIZATION", "benamaraissam");

        public static string TOKEN => _token;
        public static string PROJECT => _project;
        public static string ORGANIZATION => _organization;

        private static string GetEnvironmentVariable(string envVarName, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(envVarName);

            if (string.IsNullOrEmpty(value))
            {
                // Log warning when using default value
                Debug.WriteLine($"WARNING: Environment variable '{envVarName}' not set. Using default value: {defaultValue}");
                return defaultValue;
            }

            return value;
        }
    }
}
