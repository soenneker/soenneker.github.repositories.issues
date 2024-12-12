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

    public async ValueTask<IReadOnlyList<Issue>?> GetAllIssuesForRepository(string owner, string name, CancellationToken cancellationToken = default)
    {
        GitHubClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();
        return await client.Issue.GetAllForRepository(owner, name).NoSync();
    }

    public async ValueTask<List<Issue>> GetAllIssuesForAllRepositories(string owner, CancellationToken cancellationToken = default)
    {
        List<Issue> result = [];

        foreach (Repository repo in await _gitHubRepositoriesUtil.GetAllForOwner(owner, cancellationToken).NoSync())
        {
            IReadOnlyList<Issue>? issues = await GetAllIssuesForRepository(owner, repo.Name, cancellationToken).NoSync();

            if (issues.Populated())
                result.AddRange(issues);
        }

        return result;
    }

    public async ValueTask LogAllIssuesForAllRepositories(string owner, bool includeDependencyIssues = false, CancellationToken cancellationToken = default)
    {
        List<Issue> issues = await GetAllIssuesForAllRepositories(owner, cancellationToken).NoSync();

        foreach (Issue issue in issues)
        {
            if (includeDependencyIssues)
            {
                _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", issue.Repository.Name, issue.Title, issue.UpdatedAt);
            }
            else
            {
                if (!issue.Title.Contains("Update dependency"))
                    _logger.LogInformation("{repo}: title: {title}, updated at: {opened}", issue.Repository.Name, issue.Title, issue.UpdatedAt);
            }
        }
    }
}