using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.GitHub.OpenApiClient.Models;

namespace Soenneker.GitHub.Repositories.Issues.Abstract;

/// <summary>
/// Provides utility methods for accessing and logging GitHub repository issues.
/// </summary>
public interface IGitHubRepositoriesIssuesUtil
{
    /// <summary>
    /// Retrieves all open issues from the specified repository.
    /// </summary>
    /// <param name="owner">The owner of the repository.</param>
    /// <param name="name">The name of the repository.</param>
    /// <param name="includeDependencyIssues">Whether to include dependency-related issues (e.g., Renovate).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of open issues.</returns>
    ValueTask<List<Issue>> GetAll(string owner, string name, bool includeDependencyIssues = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all open issues across all repositories owned by the specified user or organization.
    /// </summary>
    /// <param name="owner">The owner of the repositories.</param>
    /// <param name="includeDependencyIssues">Whether to include dependency-related issues (e.g., Renovate).</param>
    /// <param name="startAt">Optional filter to restrict to repositories created after this time.</param>
    /// <param name="endAt">Optional filter to restrict to repositories created before this time.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of open issues across all repositories, or null if none found.</returns>
    ValueTask<List<Issue>?> GetAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs all open issues in the specified repository to the console or logging system.
    /// </summary>
    /// <param name="owner">The owner of the repository.</param>
    /// <param name="name">The name of the repository.</param>
    /// <param name="includeDependencyIssues">Whether to include dependency-related issues (e.g., Renovate).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    ValueTask LogAll(string owner, string name, bool includeDependencyIssues = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs all open issues for all repositories owned by the specified user or organization.
    /// </summary>
    /// <param name="owner">The owner of the repositories.</param>
    /// <param name="includeDependencyIssues">Whether to include dependency-related issues (e.g., Renovate).</param>
    /// <param name="startAt">Optional filter to restrict to repositories created after this time.</param>
    /// <param name="endAt">Optional filter to restrict to repositories created before this time.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    ValueTask LogAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null, CancellationToken cancellationToken = default);
}
