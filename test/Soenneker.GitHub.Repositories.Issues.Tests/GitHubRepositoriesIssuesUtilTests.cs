using Soenneker.GitHub.Repositories.Issues.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.GitHub.Repositories.Issues.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class GitHubRepositoriesIssuesUtilTests : HostedUnitTest
{
    private readonly IGitHubRepositoriesIssuesUtil _util;

    public GitHubRepositoriesIssuesUtilTests(Host host) : base(host)
    {
        _util = Resolve<IGitHubRepositoriesIssuesUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
