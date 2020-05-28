using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await _client.GetVariableGroupAsync(Credentials.PROJECT, id);
        }

        public async Task<List<VariableGroup>> GetVariableGroups()
        {
            return await _client.GetVariableGroupsAsync(Credentials.PROJECT);
        }

        public async Task<VariableGroup> AddVariableGroup(VariableGroup variableGroup)
        {
            return await _client.AddVariableGroupAsync(Credentials.PROJECT, variableGroup);
        }

        public async Task<VariableGroup> UpdateVariableGroup(int id,VariableGroup variableGroup)
        {
            return await _client.UpdateVariableGroupAsync(Credentials.PROJECT, id, variableGroup);
        }

        public async Task DeleteVariableGroup(int id)
        {
             await _client.DeleteVariableGroupAsync(Credentials.PROJECT, id);
        }

    }
}
