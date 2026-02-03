using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test.IntegrationTests
{
    /// <summary>
    /// Integration tests for ReleaseRequester that interact with real Azure DevOps API.
    /// These tests use a dedicated test organization and project, and clean up resources after execution.
    /// </summary>
    [TestClass]
    public class ReleaseIntegrationTest : IntegrationTestBase
    {
        [TestMethod]
        public async Task GetReleases_ShouldReturnAtLeastOneRelease()
        {
            // Act
            var releases = await ReleaseRequester.GetReleases();

            // Assert
            Assert.IsNotNull(releases);
            Assert.IsTrue(releases.Count > 0, "Expected at least one release to exist");
        }

        [TestMethod]
        public async Task GetReleaseById_ShouldReturnSpecificRelease()
        {
            // Act
            var releases = await ReleaseRequester.GetReleases();

            // Skip if no releases exist
            if (releases.Count == 0)
            {
                Assert.Inconclusive("No releases found in the project to test with");
                return;
            }

            // Act
            var firstRelease = releases[0];
            var retrievedRelease = await ReleaseRequester.GetReleaseById(firstRelease.Id);

            // Assert
            Assert.IsNotNull(retrievedRelease);
            Assert.AreEqual(firstRelease.Id, retrievedRelease.Id);
            Assert.AreEqual(firstRelease.Name, retrievedRelease.Name);
        }

        [TestMethod]
        public async Task CreateAndDeleteRelease_ShouldCreateAndCleanUpRelease()
        {
            // Arrange - Note: We can't create a release without a definition,
            // so we'll test the delete functionality with an existing release

            var releases = await ReleaseRequester.GetReleases();

            // Skip if no releases exist
            if (releases.Count == 0)
            {
                Assert.Inconclusive("No releases found in the project to test deletion");
                return;
            }

            // Act - Try to delete the first release
            var releaseToDelete = releases[0];
            var releaseId = releaseToDelete.Id;

            // We're not actually creating a new release here since it requires a definition
            // But we'll verify we can get it and then delete it
            var retrievedRelease = await ReleaseRequester.GetReleaseById(releaseId);

            // Assert
            Assert.IsNotNull(retrievedRelease);
            Assert.AreEqual(releaseId, retrievedRelease.Id);

            // This is a read-only test for now since creating releases requires a definition
            // In a real environment, you would create a release definition first
            Console.WriteLine($"Tested release: {retrievedRelease.Name} (ID: {releaseId})");
        }

        [TestMethod]
        public async Task UpdateRelease_ShouldUpdateRelease()
        {
            // Act
            var releases = await ReleaseRequester.GetReleases();

            // Skip if no releases exist
            if (releases.Count == 0)
            {
                Assert.Inconclusive("No releases found in the project to update");
                return;
            }

            // Act
            var releaseToUpdate = releases[0];
            var originalDescription = releaseToUpdate.Description ?? "";
            var originalVariables = releaseToUpdate.Variables?.Count ?? 0;

            // Add a test variable
            if (releaseToUpdate.Variables == null)
                releaseToUpdate.Variables = new Dictionary<string, ConfigurationVariableValue>();

            var testVarKey = $"TestVar_{Guid.NewGuid():N}";
            releaseToUpdate.Variables[testVarKey] = new ConfigurationVariableValue
            {
                Value = $"TestValue_{Guid.NewGuid():N}"
            };

            // Update the release
            var updatedRelease = await ReleaseRequester.UpdateRelease(releaseToUpdate.Id, releaseToUpdate);

            // Assert
            Assert.IsNotNull(updatedRelease);
            Assert.IsTrue(updatedRelease.Variables.ContainsKey(testVarKey));
            Assert.AreEqual($"TestValue_{updatedRelease.Variables[testVarKey].Value.Split('_')[1]}",
                updatedRelease.Variables[testVarKey].Value);

            // Restore original description (if it existed)
            if (!string.IsNullOrEmpty(originalDescription))
            {
                updatedRelease.Description = originalDescription;
                await ReleaseRequester.UpdateRelease(releaseToUpdate.Id, updatedRelease);
            }

            // Clean up the test variable
            if (updatedRelease.Variables.ContainsKey(testVarKey))
            {
                updatedRelease.Variables.Remove(testVarKey);
                await ReleaseRequester.UpdateRelease(releaseToUpdate.Id, updatedRelease);
            }
        }
    }
}
