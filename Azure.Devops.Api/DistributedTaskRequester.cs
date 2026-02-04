using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure.Devops.Api.Exceptions;

namespace Azure.Devops.Api.Requesters
{
    public class DistributedTaskRequester
    {
        private string Url => $"https://dev.azure.com/";
        private readonly TaskAgentHttpClient _client;

        public DistributedTaskRequester()
        {

            VssCredentials creds = new VssBasicCredential(string.Empty, Credentials.TOKEN);

            VssConnection connection = new VssConnection(new Uri($"{Url}/{Credentials.ORGANIZATION}"), creds);

            _client = connection.GetClient<TaskAgentHttpClient>();
        }

        public async Task<VariableGroup> GetVariableGroupById(int id)
        {
            try
            {
                return await _client.GetVariableGroupAsync(Credentials.PROJECT, id);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to get variable group with ID {id}: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/distributedtask/variablegroups/{id}",
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

        public async Task<List<VariableGroup>> GetVariableGroups()
        {
            try
            {
                return await _client.GetVariableGroupsAsync(Credentials.PROJECT);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to get variable groups: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/distributedtask/variablegroups",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT
                );
            }
        }

        public async Task<VariableGroup> AddVariableGroup(VariableGroup variableGroup)
        {
            try
            {
                return await _client.AddVariableGroupAsync(Credentials.PROJECT, variableGroup);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to add variable group: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/distributedtask/variablegroups",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT
                );
            }
        }

        public async Task<VariableGroup> UpdateVariableGroup(int id, VariableGroup variableGroup)
        {
            try
            {
                return await _client.UpdateVariableGroupAsync(Credentials.PROJECT, id, variableGroup);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to update variable group with ID {id}: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/distributedtask/variablegroups/{id}",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT,
                    id.ToString()
                );
            }
        }

        public async Task DeleteVariableGroup(int id)
        {
            try
            {
                await _client.DeleteVariableGroupAsync(Credentials.PROJECT, id);
            }
            catch (Exception ex)
            {
                throw new AzureDevOpsException(
                    $"Failed to delete variable group with ID {id}: {ex.Message}",
                    GetHttpStatusCode(ex),
                    GetErrorCode(ex),
                    $"https://dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis/distributedtask/variablegroups/{id}",
                    Credentials.ORGANIZATION,
                    Credentials.PROJECT,
                    id.ToString()
                );
            }
        }
    }
}
