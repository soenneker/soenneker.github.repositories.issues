using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.GitHub.Repositories.Issues.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.GitHub.Repositories.Issues.Tests;

[Collection("Collection")]
public class GitHubRepositoriesIssuesUtilTests : FixturedUnitTest
{
    private readonly IGitHubRepositoriesIssuesUtil _util;

    public GitHubRepositoriesIssuesUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IGitHubRepositoriesIssuesUtil>(true);
    }
}
