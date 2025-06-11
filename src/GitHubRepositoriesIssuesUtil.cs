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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.GitHub.Repositories.Issues;

///<inheritdoc cref="IGitHubRepositoriesIssuesUtil"/>
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
                                                    config.QueryParameters.State = "open";
                                                    config.QueryParameters.PerPage = 100;
                                                    config.QueryParameters.Page = page;
                                                }, cancellationToken)
                                                .NoSync();

            issues = response?.ToList() ?? [];

            foreach (Issue issue in issues)
            {
                if (includeDependencyIssues)
                {
                    allIssues.Add(issue);
                }
                else
                {
                    if (!issue.Title.Contains("Update dependency"))
                        allIssues.Add(issue);
                }
            }

            page++;
        } while (issues.Count > 0 && !cancellationToken.IsCancellationRequested);

        return allIssues;
    }

    public async ValueTask<List<Issue>?> GetAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null,
        CancellationToken cancellationToken = default)
    {
        List<MinimalRepository> repositories = await _gitHubRepositoriesUtil.GetAllForOwner(owner, startAt, endAt, cancellationToken).NoSync();

        if (!repositories.Any())
            return null;

        List<Issue>? result = null;

        foreach (MinimalRepository repo in repositories)
        {
            List<Issue> issues = await GetAll(owner, repo.Name, includeDependencyIssues, cancellationToken).NoSync();

            if (issues.Count == 0)
                continue;

            result ??= [];
            result.AddRange(issues);
        }

        return result;
    }

    public async ValueTask LogAll(string owner, string name, bool includeDependencyIssues = true, CancellationToken cancellationToken = default)
    {
        List<Issue> issues = await GetAll(owner, name, includeDependencyIssues, cancellationToken).NoSync();

        if (!issues.Any())
            return;

        foreach (Issue issue in issues)
        {
            _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", name, issue.Title, issue.UpdatedAt);
        }
    }

    public async ValueTask LogAllForOwner(string owner, bool includeDependencyIssues = true, DateTime? startAt = null, DateTime? endAt = null,
        CancellationToken cancellationToken = default)
    {
        List<MinimalRepository> repositories = await _gitHubRepositoriesUtil.GetAllForOwner(owner, startAt, endAt, cancellationToken).NoSync();

        if (!repositories.Any())
            return;

        foreach (MinimalRepository repo in repositories)
        {
            List<Issue> issues = await GetAll(owner, repo.Name, includeDependencyIssues, cancellationToken).NoSync();

            if (issues.Count == 0)
                continue;

            foreach (Issue issue in issues)
            {
                _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", repo.Name, issue.Title, issue.UpdatedAt);
            }
        }
    }
}