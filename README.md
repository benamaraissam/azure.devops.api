# Azure DevOps API

This repository contains a .NET library for interacting with Azure DevOps APIs, including both unit tests and integration tests.

## Project Structure

```
azure.devops.api/
├── Azure.Devops.Api/             # Main API implementation
│   ├── DistributedTaskRequester.cs   # API client for variable groups and tasks
│   ├── ReleaseRequester.cs           # API client for releases
│   └── Credentials.cs                # (Legacy) Hardcoded credentials (deprecated)
├── Azure.Devops.Api.Test/          # Test project
│   ├── IntegrationTests/             # Integration tests with real API interactions
│   │   ├── IntegrationTestBase.cs    # Base class with resource cleanup
│   │   ├── DistributedTaskIntegrationTest.cs
│   │   ├── ReleaseIntegrationTest.cs
│   │   └── README.md                 # Setup instructions for integration tests
│   ├── DistributedTaskRequesterTest.cs  # Unit tests
│   ├── ReleaseRequesterTest.cs        # Unit tests
│   ├── HttpReleaseRequesterTest.cs    # Legacy HTTP unit tests
│   └── TestCredentials.cs             # Environment variable-based credentials
└── Azure.Devops.Api.sln            # Solution file
```

## Testing Framework

This project includes two types of tests:

### Unit Tests
- Located in `Azure.Devops.Api.Test`
- Use MSTest framework
- Test individual components in isolation
- Examples: `DistributedTaskRequesterTest.cs`, `ReleaseRequesterTest.cs`

### Integration Tests
- Located in `Azure.Devops.Api.Test/IntegrationTests`
- Interact with real Azure DevOps API endpoints
- Validate end-to-end functionality
- Require environment variables for authentication (see `IntegrationTests/README.md` for setup)
- Automatically clean up created resources after execution

## Environment Variables for Integration Tests

For integration tests to work, set the following environment variables:

- `AZURE_DEVOPS_TOKEN`: Personal Access Token with appropriate permissions
- `AZURE_DEVOPS_ORGANIZATION`: Azure DevOps organization name
- `AZURE_DEVOPS_PROJECT`: Azure DevOps project name
- `AZURE_DEVOPS_TEST_PROJECT` (optional): Dedicated test project to avoid modifying production resources

See `IntegrationTests/README.md` for detailed setup instructions.

## Running Tests

### Unit Tests
```bash
cd azure.devops.api/Azure.Devops.Api.Test
dotnet test
```

### Integration Tests
1. Set environment variables as described above
2. Run tests:
```bash
cd azure.devops.api/Azure.Devops.Api.Test
dotnet test
```

## Security Note

The `Credentials.cs` file contains hardcoded credentials for legacy purposes. All new code should use `TestCredentials.cs` with environment variables to avoid exposing sensitive information.
```
