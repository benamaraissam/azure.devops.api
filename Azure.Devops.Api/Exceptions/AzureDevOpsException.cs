using System;

namespace Azure.Devops.Api.Exceptions
{
    /// <summary>
    /// Custom exception for Azure DevOps API errors
    /// Provides detailed information about API failures including HTTP status, error code, and request context
    /// </summary>
    public class AzureDevOpsException : Exception
    {
        public int HttpStatusCode { get; }
        public string ErrorCode { get; }
        public string Endpoint { get; }
        public string Organization { get; }
        public string Project { get; }
        public string ResourceId { get; }

        public AzureDevOpsException(
            string message,
            int httpStatusCode,
            string errorCode,
            string endpoint,
            string organization,
            string project,
            string resourceId = null)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            ErrorCode = errorCode;
            Endpoint = endpoint;
            Organization = organization;
            Project = project;
            ResourceId = resourceId;
        }

        public override string ToString()
        {
            var result = base.ToString();
            result += $"\nHTTP Status: {HttpStatusCode}";
            result += $"\nError Code: {ErrorCode}";
            result += $"\nEndpoint: {Endpoint}";
            result += $"\nOrganization: {Organization}";
            result += $"\nProject: {Project}";
            if (!string.IsNullOrEmpty(ResourceId))
                result += $"\nResource ID: {ResourceId}";

            return result;
        }
    }
}
