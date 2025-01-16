using System.Threading.Tasks;
using System.Threading;
using Octokit;
using System.Collections.Generic;

namespace Soenneker.GitHub.Repositories.Issues.Abstract;

/// <summary>
/// A utility library for GitHub repository Issue related operations.
/// Provides methods for retrieving and logging issues for repositories owned by a specified GitHub user or organization.
/// </summary>
public interface IGitHubRepositoriesIssuesUtil
{
    /// <summary>
    /// Retrieves all issues for a specific repository owned by the specified owner.
    /// </summary>
    /// <param name="owner">The owner of the repository for which to retrieve issues.</param>
    /// <param name="name">The name of the repository from which to retrieve issues.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing a list of issues for the specified repository, or <c>null</c> if no issues are found or the operation is canceled.
    /// </returns>
    ValueTask<IReadOnlyList<Issue>> GetAll(string owner, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all issues for all repositories owned by the specified owner.
    /// </summary>
    /// <param name="owner">The owner of the repositories for which to retrieve issues.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing a list of issues for all repositories, or <c>null</c> if no issues are found or the operation is canceled.
    /// </returns>
    ValueTask<List<Issue>?> GetAllForOwner(string owner, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs all issues for a specific repository owned by the specified owner.
    /// </summary>
    /// <param name="owner">The owner of the repository for which to log issues.</param>
    /// <param name="name">The name of the repository from which to log issues.</param>
    /// <param name="includeDependencyIssues">
    /// A boolean value indicating whether to include dependency-related issues in the log. 
    /// Defaults to <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask LogAll(string owner, string name, bool includeDependencyIssues = false, CancellationToken cancellationToken = default);

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
    ValueTask LogAllForOwner(string owner, bool includeDependencyIssues = false, CancellationToken cancellationToken = default);
}
