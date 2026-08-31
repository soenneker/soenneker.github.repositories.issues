[![](https://img.shields.io/nuget/v/soenneker.github.repositories.issues.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.github.repositories.issues/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.github.repositories.issues/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.github.repositories.issues/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.github.repositories.issues.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.github.repositories.issues/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.github.repositories.issues/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.github.repositories.issues/actions/workflows/codeql.yml)

# Soenneker.GitHub.Repositories.Issues

Retrieves and logs open GitHub issues for one repository or every repository owned by a user or organization.

## Installation

```bash
dotnet add package Soenneker.GitHub.Repositories.Issues
```

## Configure and register

```json
{
  "GH": {
    "Token": "your-github-token"
  }
}
```

```csharp
using Soenneker.GitHub.Repositories.Issues.Registrars;

services.AddGitHubRepositoriesIssuesUtilAsSingleton();
```

## Retrieve issues

```csharp
List<Issue> issues = await issueUtil.GetAll(
    "example-org",
    "example-repository",
    includeDependencyIssues: false,
    cancellationToken);
```

`GetAll()` follows all pages of open results. GitHub's endpoint also returns pull requests; this package removes entries containing pull-request metadata. When `includeDependencyIssues` is `false`, titles containing `Update dependency` are excluded case-insensitively.

```csharp
List<Issue>? ownerIssues = await issueUtil.GetAllForOwner(
    "example-org",
    startAt: DateTime.UtcNow.AddMonths(-3),
    cancellationToken: cancellationToken);
```

Owner-wide retrieval first enumerates repositories within the optional creation-date window, then queries them sequentially. It returns `null` when no matching repository has an issue and a populated list otherwise.

`LogAll()` and `LogAllForOwner()` apply the same filtering and write each issue title and update time through `ILogger`.
