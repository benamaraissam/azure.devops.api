using System;
using System.Threading.Tasks;
using Azure.Devops.Api.Requesters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Devops.Api.Test.IntegrationTests
{
    /// <summary>
    /// Integration tests for DistributedTaskRequester that interact with real Azure DevOps API.
    /// These tests use a dedicated test organization and project, and clean up resources after execution.
    /// </summary>
    [TestClass]
    public class DistributedTaskIntegrationTest : IntegrationTestBase
    {
        [TestMethod]
        public async Task GetVariableGroups_ShouldReturnAtLeastOneGroup()
        {
            // Act
            var variableGroups = await DistributedTaskRequester.GetVariableGroups();

            // Assert
            Assert.IsNotNull(variableGroups);
            Assert.IsTrue(variableGroups.Count > 0, "Expected at least one variable group to exist");
        }

        [TestMethod]
        public async Task AddVariableGroup_ShouldCreateAndReturnVariableGroup()
        {
            // Arrange
            var testVariableGroup = CreateTestVariableGroup();

            // Act
            var createdVariableGroup = await DistributedTaskRequester.AddVariableGroup(testVariableGroup);

            // Assert
            Assert.IsNotNull(createdVariableGroup);
            Assert.IsNotNull(createdVariableGroup.Id);
            Assert.AreEqual(testVariableGroup.Name, createdVariableGroup.Name);
            Assert.AreEqual(testVariableGroup.Description, createdVariableGroup.Description);
            Assert.AreEqual(1, createdVariableGroup.Variables.Count);

            // Register for cleanup
            RegisterCreatedVariableGroup(createdVariableGroup.Id);
        }

        [TestMethod]
        public async Task UpdateVariableGroup_ShouldUpdateVariableGroup()
        {
            // Arrange
            var testVariableGroup = CreateTestVariableGroup();
            var createdVariableGroup = await DistributedTaskRequester.AddVariableGroup(testVariableGroup);
            RegisterCreatedVariableGroup(createdVariableGroup.Id);

            // Update the variable group
            createdVariableGroup.Description = "Updated description for integration test";
            createdVariableGroup.Variables["UpdatedKey"] = new VariableValue { Value = "UpdatedValue" };

            // Act
            var updatedVariableGroup = await DistributedTaskRequester.UpdateVariableGroup(
                createdVariableGroup.Id, createdVariableGroup);

            // Assert
            Assert.IsNotNull(updatedVariableGroup);
            Assert.AreEqual("Updated description for integration test", updatedVariableGroup.Description);
            Assert.IsTrue(updatedVariableGroup.Variables.ContainsKey("UpdatedKey"));
            Assert.AreEqual("UpdatedValue", updatedVariableGroup.Variables["UpdatedKey"].Value);
        }

        [TestMethod]
        public async Task GetVariableGroupById_ShouldReturnSpecificVariableGroup()
        {
            // Arrange
            var testVariableGroup = CreateTestVariableGroup();
            var createdVariableGroup = await DistributedTaskRequester.AddVariableGroup(testVariableGroup);
            RegisterCreatedVariableGroup(createdVariableGroup.Id);

            // Act
            var retrievedVariableGroup = await DistributedTaskRequester.GetVariableGroupById(createdVariableGroup.Id);

            // Assert
            Assert.IsNotNull(retrievedVariableGroup);
            Assert.AreEqual(createdVariableGroup.Id, retrievedVariableGroup.Id);
            Assert.AreEqual(createdVariableGroup.Name, retrievedVariableGroup.Name);
        }
    }
}
