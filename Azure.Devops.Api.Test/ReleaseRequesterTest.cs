using System;
using System.Threading.Tasks;
using Azure.Devops.Api.Requesters;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test
{
    [TestClass]
    public class ReleaseRequesterTest
    {

        private readonly ReleaseRequester _releaseRequester = new ReleaseRequester();

        [TestMethod]
        public async Task GetReleases()
        {
            var result = await _releaseRequester.GetReleases();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public async Task UpdateRelease()
        {
            var release = await _releaseRequester.GetReleaseById(1);
            var key = UpdateItem(release);
            var response = await _releaseRequester.UpdateRelease(1, release);
            Assert.IsTrue(response.Variables.ContainsKey(key));
        }

        private string UpdateItem(Release release)
        {
            var key = Guid.NewGuid().ToString();
            release.Variables.Add(key, new ConfigurationVariableValue { Value = Guid.NewGuid().ToString() });
            return key;
        }
    }
}
