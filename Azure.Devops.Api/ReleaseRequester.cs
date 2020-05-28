using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await _client.GetReleaseAsync(Credentials.PROJECT, id);
        }

        public async Task<List<Release>> GetReleases()
        {
            return await _client.GetReleasesAsync(Credentials.PROJECT);
        }


        public async Task<Release> UpdateRelease(int id, Release release)
        {
            return await _client.UpdateReleaseAsync(release, Credentials.PROJECT, id);
        }

        public async Task DeleteRelease(int id)
        {
            await _client.DeleteReleaseAsync(Credentials.PROJECT, id);
        }

    }
}
