using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure.Devops.Api.Exceptions;

namespace Azure.Devops.Api.Requesters
{
    public class ReleaseRequester
    {
        private string Url => $"https://vsrm.dev.azure.com/";
        private readonly ReleaseHttpClient _client;

        public ReleaseRequester()
        {
            VssCredentials creds = new VssBasicCredential(string.Empty, Credentials.TOKEN);

            VssConnection connection = new VssConnection(new Uri($"{Url}/{Credentials.ORGANIZATION}"), creds);

            _client = connection.GetClient<ReleaseHttpClient>();
        }

        public async Task<Release> GetReleaseById(int id)
        {
            try
            {
                return await _client.GetReleaseAsync(Credentials.PROJECT, id);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to get release with ID {id}: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://vsrm.dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/release/releases/{id}",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT,
                    id.ToString()
                );
            }
        }

        public async Task<List<Release>> GetReleases()
        {
            try
            {
                return await _client.GetReleasesAsync(Credentials.PROJECT);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to get releases: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://vsrm.dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/release/releases",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT
                );
            }
        }

        public async Task<Release> UpdateRelease(int id, Release release)
        {
            try
            {
                return await _client.UpdateReleaseAsync(release, Credentials.PROJECT, id);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to update release with ID {id}: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://vsrm.dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/release/releases/{id}",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT,
                    id.ToString()
                );
            }
        }

        public async Task DeleteRelease(int id)
        {
            try
            {
                await _client.DeleteReleaseAsync(Credentials.PROJECT, id);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to delete release with ID {id}: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://vsrm.dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/release/releases/{id}",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT,
                    id.ToString()
                );
            }
        }

        private int GetHttpStatusCode(Exception ex)
        {
            if (ex is Microsoft.VisualStudio.Services.WebApi.VssServiceException serviceEx)
            {
                return serviceEx.ResponseStatusCode;
            }
            else if (ex is System.Net.WebException webEx)
            {
                if (webEx.Response is HttpWebResponse httpRes)
                {
                    return (int)httpRes.StatusCode;
                }
            }
            return 500; // Internal Server Error as default
        }

        private string GetErrorCode(Exception ex)
        {
            if (ex is Microsoft.VisualStudio.Services.WebApi.VssServiceException serviceEx)
            {
                return serviceEx.ErrorCode ?? "Unknown";
            }
            else if (ex.Message.Contains("404"))
            {
                return "NotFound";
            }
            else if (ex.Message.Contains("401"))
            {
                return "Unauthorized";
            }
            else if (ex.Message.Contains("403"))
            {
                return "Forbidden";
            }
            else if (ex.Message.Contains("400"))
            {
                return "BadRequest";
            }
            else if (ex.Message.Contains("500"))
            {
                return "InternalServerError";
            }
            return "Unknown";
        }
    }
}
