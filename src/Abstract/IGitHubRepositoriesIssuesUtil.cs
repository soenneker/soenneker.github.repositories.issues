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
    ValueTask<IReadOnlyList<Issue>?> GetAllIssuesForRepository(string owner, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all issues for all repositories owned by the specified owner.
    /// </summary>
    /// <param name="owner">The owner of the repositories for which to retrieve issues.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing a list of issues for all repositories.
    /// </returns>
    ValueTask<List<Issue>> GetAllIssuesForAllRepositories(string owner, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs all issues for all repositories owned by the specified owner.
    /// </summary>
    /// <param name="owner">The owner of the repositories for which to log issues.</param>
    /// <param name="includeDependencyIssues">
    /// A boolean value indicating whether to include dependency-related issues in the log. 
    /// Defaults to <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask LogAllIssuesForAllRepositories(string owner, bool includeDependencyIssues = false, CancellationToken cancellationToken = default);
}
