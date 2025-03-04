using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Common;
using Sidekick.Common.Extensions;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Query;

[Collection(Collections.Poe2Parser)]
public class TradeSearchServiceTests
{
    private readonly IItemParser parser;
    private readonly ITradeFilterService tradeFilterService;
    private readonly MockHttpClient mockHttpClient = new();
    private readonly TradeSearchService tradeSearchService;

    public TradeSearchServiceTests(ParserFixture fixture)
    {
        parser = fixture.Parser;
        tradeFilterService = fixture.TradeFilterService;

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(mockHttpClient);

        tradeSearchService = new TradeSearchService(
            NullLogger<TradeSearchService>.Instance,
            fixture.GameLanguageProvider,
            fixture.SettingsService,
            fixture.ModifierProvider,
            fixture.FilterProvider,
            fixture.PropertyParser,
            mockFactory.Object
        );
    }

    [Fact]
    public async Task ExpectInputYieldsIsNotIdentified()
    {
        var input = ResourceHelper.ReadFileContent("TimeLostDiamond/item.txt");

        var parsedItem = parser.ParseItem(input);
        var propertyFilters = await tradeFilterService.GetPropertyFilters(parsedItem);
        var result = await tradeSearchService.Search(parsedItem, propertyFilters);
        Assert.Null(result.Error);

        var actual = Assert.Single(mockHttpClient.Requests);
        var actualQuery = actual.FromJsonTo<Trade.Requests.QueryRequest>(SerializationOptions.WithEnumConverterOptions);

        var identified = actualQuery?.Query.Filters.MiscFilters?.Filters.Identified?.Option;
        Assert.Equal("false", identified);
    }

    [Fact]
    public void ExpectQueryIsNotIdentified()
    {
        var expectedQuery = ResourceHelper.ReadFileContent("TimeLostDiamond/query.json");
        var queryRequest = expectedQuery.FromJsonTo<Trade.Requests.QueryRequest>(SerializationOptions.WithEnumConverterOptions);

        var identified = queryRequest?.Query.Filters.MiscFilters?.Filters.Identified?.Option;
        Assert.Equal("false", identified);
    }

    [Fact]
    public void ExpectSerializationDeserializationNoChanges()
    {
        var expected = ResourceHelper.ReadFileContent("TimeLostDiamond/query.json");
        var queryRequest = expected.FromJsonTo<Trade.Requests.QueryRequest>(SerializationOptions.WithEnumConverterOptions);

        var actual = queryRequest.ToJson(SerializationOptions.WithEnumConverterOptions);

        Assert.Equal(actual, expected);
    }

    [Theory]
    [InlineData("TimeLostDiamond/item.txt", "TimeLostDiamond/query.json")]
    public async Task InputShouldCreateExpectedQuery(string pathToInput, string pathToExpectedQuery)
    {
        var input = ResourceHelper.ReadFileContent(pathToInput);
        var expectedQuery = ResourceHelper.ReadFileContent(pathToExpectedQuery);

        var parsedItem = parser.ParseItem(input);
        var propertyFilters = await tradeFilterService.GetPropertyFilters(parsedItem);
        var modifierFilters = tradeFilterService.GetModifierFilters(parsedItem);
        var pseudoModifierFilters = tradeFilterService.GetPseudoModifierFilters(parsedItem);
        var result = await tradeSearchService.Search(parsedItem, propertyFilters, modifierFilters, pseudoModifierFilters);
        Assert.Null(result.Error);

        var actual = Assert.Single(mockHttpClient.Requests);

        Assert.Equal(actual, expectedQuery);
    }
}
