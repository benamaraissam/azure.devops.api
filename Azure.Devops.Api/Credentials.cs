using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Azure.Devops.Api
{
    public static class Credentials
    {
        private static string _organization;
        private static string _project;
        private static string _token;

        public static string Organization
        {
            get
            {
                if (_organization != null) return _organization;
                _organization = GetSetting("AzureDevOps:Organization");
                if (string.IsNullOrEmpty(_organization))
                    throw new InvalidOperationException("Azure DevOps organization is not configured. Set environment variable AZURE_DEVOPS_ORGANIZATION or add organization to appsettings.json.");
                return _organization;
            }
        }

        public static string Project
        {
            get
            {
                if (_project != null) return _project;
                _project = GetSetting("AzureDevOps:Project");
                if (string.IsNullOrEmpty(_project))
                    throw new InvalidOperationException("Azure DevOps project is not configured. Set environment variable AZURE_DEVOPS_PROJECT or add project to appsettings.json.");
                return _project;
            }
        }

        public static string Token
        {
            get
            {
                if (_token != null) return _token;
                _token = GetSetting("AzureDevOps:Token");
                if (string.IsNullOrEmpty(_token))
                    throw new InvalidOperationException("Azure DevOps personal access token is not configured. Set environment variable AZURE_DEVOPS_TOKEN or add token to appsettings.json.");
                return _token;
            }
        }

        private static string GetSetting(string key)
        {
            // First try environment variables
            var envVar = key.Replace(":", "_").ToUpper();
            var envValue = Environment.GetEnvironmentVariable(envVar);
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            // If environment variables not set, try appsettings.json
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
                return null;

            try
            {
                var json = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                // Navigate nested JSON structure
                var azureDevOps = config["AzureDevOps"] as Dictionary<string, object>;
                if (azureDevOps == null)
                    return null;

                var value = azureDevOps[key.Split(':')[1]];
                return value?.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading configuration from appsettings.json: {ex.Message}");
            }
        }
    }
}
