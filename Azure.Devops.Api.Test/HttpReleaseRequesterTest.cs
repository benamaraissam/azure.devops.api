using System;
using System.Threading.Tasks;
using Azure.Devops.Api.Legacy;
using Azure.Devops.Api.Requesters;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test
{
    [TestClass]
    public class HttpReleaseRequesterTest
    {

        private readonly HttpReleaseRequester _releaseRequester = new HttpReleaseRequester();

        [TestMethod]
        public async Task GetReleaseById()
        {
            var result = await _releaseRequester.GetReleaseById(1);
            //Assert.IsTrue(result.Variables.Count > 0);
        }

        [TestMethod]
        public async Task GetReleases()
        {
            var result = await _releaseRequester.GetReleases();
            Assert.IsTrue(result.Value.Count > 0); ;
        }

        [TestMethod]
        public async Task UpdateRelease()
        {
            var result = await _releaseRequester.GetReleaseById(1);
            result.Variables.Add(Guid.NewGuid().ToString(), new Models.VariableValue { Value = Guid.NewGuid().ToString() });
            var response = await _releaseRequester.UpdateRelease(result);
        }
    }
}
