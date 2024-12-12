using System.Threading.Tasks;
using System.Threading;
using Octokit;
using System.Collections.Generic;

namespace Soenneker.GitHub.Repositories.Issues.Abstract;

/// <summary>
/// A utility library for GitHub repository Issue related operations
/// </summary>
public interface IGitHubRepositoriesIssuesUtil
{
    ValueTask<List<Issue>> GetAllIssuesForAllRepositories(string owner, CancellationToken cancellationToken = default);

    ValueTask LogAllIssuesForAllRepositories(string owner, bool includeDependencyIssues = false, CancellationToken cancellationToken = default);
}
