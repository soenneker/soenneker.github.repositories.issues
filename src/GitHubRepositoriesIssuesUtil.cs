using Soenneker.GitHub.Repositories.Issues.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Octokit;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.GitHub.Repositories.Abstract;
using Soenneker.GitHub.Client.Abstract;
using Soenneker.Extensions.Enumerable;
using System;

namespace Soenneker.GitHub.Repositories.Issues;

/// <inheritdoc cref="IGitHubRepositoriesIssuesUtil"/>
public class GitHubRepositoriesIssuesUtil : IGitHubRepositoriesIssuesUtil
{
    private readonly ILogger<GitHubRepositoriesIssuesUtil> _logger;
    private readonly IGitHubRepositoriesUtil _gitHubRepositoriesUtil;
    private readonly IGitHubClientUtil _gitHubClientUtil;

    public GitHubRepositoriesIssuesUtil(ILogger<GitHubRepositoriesIssuesUtil> logger, IGitHubRepositoriesUtil gitHubRepositoriesUtil, IGitHubClientUtil gitHubClientUtil)
    {
        _logger = logger;
        _gitHubRepositoriesUtil = gitHubRepositoriesUtil;
        _gitHubClientUtil = gitHubClientUtil;
    }

    public async ValueTask<IReadOnlyList<Issue>> GetAll(string owner, string name, bool includeDependencyIssues = true, CancellationToken cancellationToken = default)
    {
        GitHubClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();

        var allIssues = new List<Issue>();
        var page = 1;
        IReadOnlyList<Issue> issues;

        var repositoryIssueRequest = new RepositoryIssueRequest { State = ItemStateFilter.Open };

        do
        {
            var options = new ApiOptions
            {
                PageCount = 1,
                PageSize = 100, // GitHub API default max page size
                StartPage = page
            };

            issues = await client.Issue.GetAllForRepository(owner, name, repositoryIssueRequest, options).NoSync();

            foreach (Issue issue in issues)
            {
                if (includeDependencyIssues)
                {
                    allIssues.Add(issue);
                }
                else
                {
                    if (!issue.Title.Contains("Update dependency")) // renovate, dependabot needs help
                        allIssues.Add(issue);
                }
            }

            page++;
        } while (issues.Count > 0 && !cancellationToken.IsCancellationRequested);

        return allIssues;
    }

    public async ValueTask<List<Issue>?> GetAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Repository> repositories = await _gitHubRepositoriesUtil.GetAllForOwner(owner, null, endAt, cancellationToken).NoSync();

        if (repositories.Count == 0)
            return null;

        List<Issue>? result = null;

        foreach (Repository repo in repositories)
        {
            IReadOnlyList<Issue> issues = await GetAll(owner, repo.Name, includeDependencyIssues, cancellationToken).NoSync();

            if (issues.Populated())
            {
                result ??= [];
                result.AddRange(issues);
            }
        }

        return result;
    }

    public async ValueTask LogAll(string owner, string name, bool includeDependencyIssues = true, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Issue> issues = await GetAll(owner, name, includeDependencyIssues, cancellationToken).NoSync();

        if (issues.IsNullOrEmpty())
            return;

        foreach (Issue issue in issues)
        {
            _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", name, issue.Title, issue.UpdatedAt);
        }
    }

    public async ValueTask LogAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Repository> repositories = await _gitHubRepositoriesUtil.GetAllForOwner(owner, null, endAt, cancellationToken).NoSync();

        if (repositories.Count == 0)
            return;

        foreach (Repository repo in repositories)
        {
            IReadOnlyList<Issue> issues = await GetAll(owner, repo.Name, includeDependencyIssues, cancellationToken).NoSync();

            if (issues.IsNullOrEmpty())
                continue;

            foreach (Issue issue in issues)
            {
                _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", repo.Name, issue.Title, issue.UpdatedAt);
            }
        }
    }
}