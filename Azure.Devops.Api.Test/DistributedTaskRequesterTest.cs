using System;
using System.Threading.Tasks;
using Azure.Devops.Api.Requesters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test
{
    [TestClass]
    public class DistributedTaskRequesterTest
    {
        private readonly DistributedTaskRequester _distributedTask = new DistributedTaskRequester();

        [TestMethod]
        public async Task GetVariableGroups()
        {
            var result = await _distributedTask.GetVariableGroups();
            Assert.IsTrue(result.Count > 0);
        }



        [TestMethod]
        public async Task AddVariableGroup()
        {
            var result = await _distributedTask.AddVariableGroup(CreateVariableGroup());
            Assert.IsTrue(result.Variables.Count == 1);
        }

        private VariableGroup CreateVariableGroup(int id = 0)
        {
            return new VariableGroup
            {
                Id = id,
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Variables = new System.Collections.Generic.Dictionary<string, VariableValue>
                {
                    {
                        Guid.NewGuid().ToString(),
                        new VariableValue
                        {
                            Value = Guid.NewGuid().ToString()
                        }
                    }
                }
            };
        }
    }
}
