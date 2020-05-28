using Azure.Devops.Api.Models;
using System.Threading.Tasks;

namespace Azure.Devops.Api.Legacy
{
    public class HttpReleaseRequester : RestRequester
    {
        protected override string Url => $"https://vsrm.dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis";

        private string _endpoint => "release/releases";
        public async Task<Release> GetReleaseById(int id)
        {
            return await Get<Release>($"{_endpoint}/{id}");
        }

        public async Task<Releases> GetReleases()
        {
            return await Get<Releases>($"{_endpoint}");
        }

    }
}
