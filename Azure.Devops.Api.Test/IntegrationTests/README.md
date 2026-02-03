# Azure DevOps Integration Test Setup

## Overview
This directory contains integration tests that interact with real Azure DevOps services. These tests validate the actual API interactions rather than using mocks.

## Prerequisites
Before running these tests, you need to configure the following environment variables:

### Required Environment Variables
| Variable | Description |
|----------|-------------|
| `AZURE_DEVOPS_TOKEN` | Personal Access Token (PAT) with appropriate permissions for your Azure DevOps organization |
| `AZURE_DEVOPS_ORGANIZATION` | Your Azure DevOps organization name (e.g., "your-organization") |
| `AZURE_DEVOPS_PROJECT` | Your Azure DevOps project name (e.g., "your-project") |

### Optional Environment Variable
| Variable | Description |
|----------|-------------|
| `AZURE_DEVOPS_TEST_PROJECT` | Optional: A dedicated test project for integration tests. If not specified, the main project will be used. |

## Setting Up Environment Variables

### Windows (Command Prompt)
```cmd
set AZURE_DEVOPS_TOKEN=your_personal_access_token_here
set AZURE_DEVOPS_ORGANIZATION=your-organization
set AZURE_DEVOPS_PROJECT=your-project
```

### Windows (PowerShell)
```powershell
$env:AZURE_DEVOPS_TOKEN="your_personal_access_token_here"
$env:AZURE_DEVOPS_ORGANIZATION="your-organization"
$env:AZURE_DEVOPS_PROJECT="your-project"
```

### Linux/macOS
```bash
export AZURE_DEVOPS_TOKEN="your_personal_access_token_here"
export AZURE_DEVOPS_ORGANIZATION="your-organization"
export AZURE_DEVOPS_PROJECT="your-project"
```

## Creating a Personal Access Token (PAT)

1. Go to your Azure DevOps organization: `https://dev.azure.com/your-organization`
2. Click on your profile picture in the top-right corner
3. Select "Personal access tokens"
4. Click "New Token"
5. Configure the token with:
   - **Organization**: Your organization
   - **Token name**: "Integration Test Token"
   - **Expiration**: Choose an appropriate duration
   - **Scopes**: Select at least:
     - `Variable groups (read and manage)`
     - `Release (read and manage)`
     - `Build (read and manage)`
     - `Project and team (read)`
6. Click "Create"
7. Copy the generated token and use it as your `AZURE_DEVOPS_TOKEN`

## Running the Tests

You can run the integration tests using any of the following methods:

### Visual Studio
1. Open the solution in Visual Studio
2. Go to Test > Run > All Tests

### Command Line (dotnet)
```bash
cd azure.devops.api/Azure.Devops.Api.Test
dotnet test
```

### Command Line (vstest)
```bash
cd azure.devops.api/Azure.Devops.Api.Test
vstest.console.exe bin/Debug/Azure.Devops.Api.Test.dll
```

## Important Notes

1. **Resource Cleanup**: All integration tests automatically clean up resources they create after execution. This includes variable groups and releases.

2. **Test Project**: For production environments, it's recommended to use a separate test project by setting `AZURE_DEVOPS_TEST_PROJECT` to avoid modifying production resources.

3. **Permissions**: The PAT needs sufficient permissions to create, update, and delete variable groups and releases.

4. **Network Connectivity**: Ensure your machine has network access to `https://dev.azure.com` and `https://vsrm.dev.azure.com`.

5. **Test Data**: The integration tests may modify existing resources in your project. Use a dedicated test project whenever possible.

## Troubleshooting

If you encounter authentication errors:
- Verify your PAT has the correct scopes
- Ensure the organization and project names are correct
- Check that your PAT hasn't expired
- Confirm network connectivity to Azure DevOps services

If cleanup fails:
- Manual cleanup of orphaned resources may be required
- Check that your PAT has delete permissions