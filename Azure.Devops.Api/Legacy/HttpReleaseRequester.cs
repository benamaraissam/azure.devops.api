using Azure.Devops.Api.Models;
using System.Threading.Tasks;

namespace Azure.Devops.Api.Legacy
{
    public class HttpReleaseRequester : RestRequester
    {
        protected override string Url => $"https://vsrm.dev.azure.com/{Credentials.ORGANIZATION}/{Credentials.PROJECT}/_apis";

        private string _endpoint => "release/definitions";

        private string _version => "api-version=5.1-preview.1";

        public async Task<Release> GetReleaseById(int id)
        {
            return await Get<Release>($"{_endpoint}/{id}");
        }

        public async Task<Releases> GetReleases()
        {
            return await Get<Releases>($"{_endpoint}");
        }

        public async Task<Release> UpdateRelease(Release release)
        {
            return await Put<Release>($"{_endpoint}/{release.Id}?{_version}", release);
        }

    }
}
