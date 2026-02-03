using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Devops.Api.Requesters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test.IntegrationTests
{
    /// <summary>
    /// Base class for integration tests that interact with Azure DevOps.
    /// This class provides automatic cleanup of test resources.
    /// </summary>
    [TestClass]
    public abstract class IntegrationTestBase
    {
        protected DistributedTaskRequester DistributedTaskRequester { get; }
        protected ReleaseRequester ReleaseRequester { get; }

        private readonly List<int> _createdVariableGroups = new List<int>();
        private readonly List<int> _createdReleases = new List<int>();

        public IntegrationTestBase()
        {
            // Validate environment variables before creating clients
            TestCredentials.ValidateEnvironmentVariables();

            DistributedTaskRequester = new DistributedTaskRequester();
            ReleaseRequester = new ReleaseRequester();
        }

        /// <summary>
        /// Runs after each test to clean up created resources.
        /// This ensures tests don't leave behind orphaned resources.
        /// </summary>
        [TestCleanup]
        public async Task CleanupResources()
        {
            // Clean up variable groups
            foreach (var variableGroupId in _createdVariableGroups)
            {
                try
                {
                    await DistributedTaskRequester.DeleteVariableGroup(variableGroupId);
                    Console.WriteLine($"Cleaned up variable group: {variableGroupId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to delete variable group {variableGroupId}: {ex.Message}");
                }
            }
            _createdVariableGroups.Clear();

            // Clean up releases
            foreach (var releaseId in _createdReleases)
            {
                try
                {
                    await ReleaseRequester.DeleteRelease(releaseId);
                    Console.WriteLine($"Cleaned up release: {releaseId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to delete release {releaseId}: {ex.Message}");
                }
            }
            _createdReleases.Clear();
        }

        /// <summary>
        /// Records a created variable group for later cleanup.
        /// </summary>
        protected void RegisterCreatedVariableGroup(int variableGroupId)
        {
            _createdVariableGroups.Add(variableGroupId);
        }

        /// <summary>
        /// Records a created release for later cleanup.
        /// </summary>
        protected void RegisterCreatedRelease(int releaseId)
        {
            _createdReleases.Add(releaseId);
        }

        /// <summary>
        /// Creates a test variable group with a unique name.
        /// The variable group will be automatically cleaned up after the test.
        /// </summary>
        protected VariableGroup CreateTestVariableGroup()
        {
            return new VariableGroup
            {
                Name = $"TestVariableGroup_{Guid.NewGuid():N}",
                Description = "Test variable group created by integration test",
                Variables = new Dictionary<string, VariableValue>
                {
                    {
                        $"TestVar_{Guid.NewGuid():N}",
                        new VariableValue
                        {
                            Value = $"TestValue_{Guid.NewGuid():N}"
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Creates a test release with a unique name.
        /// The release will be automatically cleaned up after the test.
        /// </summary>
        protected Release CreateTestRelease()
        {
            return new Release
            {
                Name = $"TestRelease_{Guid.NewGuid():N}",
                Description = "Test release created by integration test",
                Variables = new Dictionary<string, ConfigurationVariableValue>
                {
                    {
                        $"TestVar_{Guid.NewGuid():N}",
                        new ConfigurationVariableValue
                        {
                            Value = $"TestValue_{Guid.NewGuid():N}"
                        }
                    }
                }
            };
        }
    }
}
```

Now I'll create the integration test for DistributedTaskRequester:
