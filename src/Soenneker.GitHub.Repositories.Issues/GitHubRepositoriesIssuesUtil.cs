using Microsoft.Extensions.Logging;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.GitHub.ClientUtil.Abstract;
using Soenneker.GitHub.OpenApiClient;
using Soenneker.GitHub.OpenApiClient.Models;
using Soenneker.GitHub.Repositories.Abstract;
using Soenneker.GitHub.Repositories.Issues.Abstract;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.GitHub.Repositories.Issues;

/// <inheritdoc cref="IGitHubRepositoriesIssuesUtil" />
public sealed class GitHubRepositoriesIssuesUtil : IGitHubRepositoriesIssuesUtil
{
    private readonly ILogger<GitHubRepositoriesIssuesUtil> _logger;
    private readonly IGitHubOpenApiClientUtil _gitHubClientUtil;
    private readonly IGitHubRepositoriesUtil _gitHubRepositoriesUtil;

    public GitHubRepositoriesIssuesUtil(ILogger<GitHubRepositoriesIssuesUtil> logger, IGitHubOpenApiClientUtil gitHubClientUtil,
        IGitHubRepositoriesUtil gitHubRepositoriesUtil)
    {
        _logger = logger;
        _gitHubClientUtil = gitHubClientUtil;
        _gitHubRepositoriesUtil = gitHubRepositoriesUtil;
    }

    public async ValueTask<List<Issue>> GetAll(string owner, string name, bool includeDependencyIssues = true, CancellationToken cancellationToken = default)
    {
        GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();

        var allIssues = new List<Issue>();
        var page = 1;
        List<Issue> issues;

        do
        {
                                                            List<Issue>? response = await client.Repos[owner][name]
                                                .Issues.GetAsync(config =>
                                                {
                                                    config.QueryParameters.State = IssuesListForRepoStateParameter.Open;
                                                    config.QueryParameters.PerPage = 100;
                                                    config.QueryParameters.Page = page;
                                                }, cancellationToken)
                                                .NoSync();

            issues = response ?? [];

            foreach (Issue issue in issues)
            {
                if (ShouldInclude(issue, includeDependencyIssues))
                    allIssues.Add(issue);
            }

            page++;
        } while (issues.Count > 0 && !cancellationToken.IsCancellationRequested);

        return allIssues;
    }

    public async ValueTask<List<Issue>?> GetAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null,
        CancellationToken cancellationToken = default)
    {
        List<MinimalRepository> repositories = await _gitHubRepositoriesUtil.GetAllForOwner(owner, startAt, endAt, cancellationToken).NoSync();

        if (repositories.Count == 0)
            return null;

        List<Issue>? result = null;

        foreach (MinimalRepository repo in repositories)
        {
            if (repo.Name is not { Length: > 0 } repoName)
                continue;

            List<Issue> issues = await GetAll(owner, repoName, includeDependencyIssues, cancellationToken).NoSync();

            if (issues.Count == 0)
                continue;

            result ??= [];
            result.AddRange(issues);
        }

        return result;
    }

    public async ValueTask LogAll(string owner, string name, bool includeDependencyIssues = true, CancellationToken cancellationToken = default)
    {
        GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();

        var page = 1;
        List<Issue> issues;

        do
        {
            List<Issue>? response = await client.Repos[owner][name]
                .Issues.GetAsync(config =>
                {
                    config.QueryParameters.State = IssuesListForRepoStateParameter.Open;
                    config.QueryParameters.PerPage = 100;
                    config.QueryParameters.Page = page;
                }, cancellationToken)
                .NoSync();

            issues = response ?? [];

            foreach (Issue issue in issues)
            {
                if (ShouldInclude(issue, includeDependencyIssues))
                {
                    _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", name, issue.Title, issue.UpdatedAt);
                }
            }

            page++;
        } while (issues.Count > 0 && !cancellationToken.IsCancellationRequested);
    }

    public async ValueTask LogAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null,
        CancellationToken cancellationToken = default)
    {
        List<MinimalRepository> repositories = await _gitHubRepositoriesUtil.GetAllForOwner(owner, startAt, endAt, cancellationToken).NoSync();

        if (repositories.Count == 0)
            return;

        GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();

        foreach (MinimalRepository repo in repositories)
        {
            if (string.IsNullOrEmpty(repo.Name))
                continue;

            var page = 1;
            List<Issue> issues;

            do
            {
                List<Issue>? response = await client.Repos[owner][repo.Name]
                    .Issues.GetAsync(config =>
                    {
                        config.QueryParameters.State = IssuesListForRepoStateParameter.Open;
                        config.QueryParameters.PerPage = 100;
                        config.QueryParameters.Page = page;
                    }, cancellationToken)
                    .NoSync();

                issues = response ?? [];

                foreach (Issue issue in issues)
                {
                    if (ShouldInclude(issue, includeDependencyIssues))
                    {
                        _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", repo.Name, issue.Title, issue.UpdatedAt);
                    }
                }

                page++;
            } while (issues.Count > 0 && !cancellationToken.IsCancellationRequested);
        }
    }

    private static bool ShouldInclude(Issue issue, bool includeDependencyIssues)
    {
        if (issue.PullRequest != null)
            return false;

        return includeDependencyIssues || issue.Title?.Contains("Update dependency", StringComparison.OrdinalIgnoreCase) != true;
    }
}
