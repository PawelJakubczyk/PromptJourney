using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Tests.Repositories;

public class ExampleLinksRepositoryTestsBase : BaseTransactionIntegrationTest
{
    private const string TestLink1 = "https://example.com/default-image1.jpg";
    private const string TestLink2 = "https://example.com/default-image2.jpg";
    private const string TestLink3 = "https://example.com/default-image3.jpg";

    private const string TestStyleName1 = "DefaultTestStyle1";
    private const string TestStyleName2 = "DefaultTestStyle2";
    private const string TestStyleName3 = "DefaultTestStyle3";

    private const string TestVersion1 = "1.0";
    private const string TestVersion2 = "2.0";
    private const string TestVersion3 = "3.0";

    private readonly VersionsRepository _versionsRepository;
    private readonly ExampleLinkRepository _exampleLinkRepository;
    private readonly StylesRepository _stylesRepository;

    private readonly CancellationToken _cancellationToken;

    public ExampleLinksRepositoryTestsBase(MidjourneyDbFixture fixture) : base(fixture)
    {
        _exampleLinkRepository = new ExampleLinkRepository(DbContext);
        _versionsRepository = new VersionsRepository(DbContext);
        _stylesRepository = new StylesRepository(DbContext);
    }
}
