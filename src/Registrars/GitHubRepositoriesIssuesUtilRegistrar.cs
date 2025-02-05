using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.GitHub.Repositories.Issues.Abstract;
using Soenneker.GitHub.Repositories.Registrars;

namespace Soenneker.GitHub.Repositories.Issues.Registrars;

/// <summary>
/// A utility library for GitHub repository Issue related operations
/// </summary>
public static class GitHubRepositoriesIssuesUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="IGitHubRepositoriesIssuesUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddGitHubRepositoriesIssuesUtilAsSingleton(this IServiceCollection services)
    {
        services.AddGitHubRepositoriesUtilAsSingleton()
                .TryAddSingleton<IGitHubRepositoriesIssuesUtil, GitHubRepositoriesIssuesUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IGitHubRepositoriesIssuesUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddGitHubRepositoriesIssuesUtilAsScoped(this IServiceCollection services)
    {
        services.AddGitHubRepositoriesUtilAsScoped()
                .TryAddScoped<IGitHubRepositoriesIssuesUtil, GitHubRepositoriesIssuesUtil>();

        return services;
    }
}