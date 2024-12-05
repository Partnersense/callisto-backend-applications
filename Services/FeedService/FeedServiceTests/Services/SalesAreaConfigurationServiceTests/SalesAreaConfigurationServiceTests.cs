using FeedService.Services.SalesAreaConfigurationServices;
using FeedService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SharedLib.Clients.Norce;
using SharedLib.Models.Norce.Query;
using SharedLib.Options.Models;
using SharedLib.Clients.Norce;
using System.Text.Json;

namespace FeedServiceTests.Services.SalesAreaConfigurationServiceTests;

public class SalesAreaConfigurationServiceTests
{
    private readonly Mock<ILogger<SalesAreaConfigurationService>> _loggerMock;
    private readonly Mock<INorceClient> _norceClientMock;
    private readonly Mock<INorceQueryClient> _queryClientMock;
    private readonly Mock<IOptionsMonitor<NorceBaseModuleOptions>> _optionsMonitorMock;
    private readonly SalesAreaConfigurationService _service;

    public SalesAreaConfigurationServiceTests()
    {
        _loggerMock = new Mock<ILogger<SalesAreaConfigurationService>>();
        _norceClientMock = new Mock<INorceClient>();
        _queryClientMock = new Mock<INorceQueryClient>();
        _optionsMonitorMock = new Mock<IOptionsMonitor<NorceBaseModuleOptions>>();

        // Setup Query property to return the mock query client
        _norceClientMock.Setup(x => x.Query).Returns(_queryClientMock.Object);

        _service = new SalesAreaConfigurationService(
            _loggerMock.Object,
            _norceClientMock.Object,
            _optionsMonitorMock.Object
        );
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_Success_ReturnsSalesAreaConfigurations()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        var salesAreas = new List<SalesAreaResponse>
        {
            new()
            {
                SalesAreaId = 1,
                ClientId = 1006,
                IsPrimary = true,
                Created = DateTime.UtcNow,
                CreatedBy = 1,
                Code = "SE"
            }
        };

        var priceLists = new List<PriceListClientResponse>
        {
            new()
            {
                PriceListId = 1,
                SalesAreaId = 1,
                CurrencyId = 1,
                PriceList = new PriceListResponse { IsActive = true }
            }
        };

        var currencies = new List<CurrenciesResponse>
        {
            new()
            {
                Id = 1,
                Code = "SEK",
                IsActive = true
            }
        };

        var salesAreasJson = JsonSerializer.Serialize(salesAreas);
        var priceListsJson = JsonSerializer.Serialize(priceLists);
        var currenciesJson = JsonSerializer.Serialize(currencies);

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(salesAreasJson);

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceLists, It.IsAny<string>()))
            .ReturnsAsync(priceListsJson);

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Core.Currencies, It.IsAny<string>()))
            .ReturnsAsync(currenciesJson);

        // Act
        var result = await _service.GetSalesAreaConfigurations(traceId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var config = result.First();
        Assert.Equal(1, config.SalesAreaId);
        Assert.Equal("SE", config.SalesAreaCode);
        Assert.Equal("SEK", config.CurrencyCode);
        Assert.True(config.IsPrimary);
        Assert.Single(config.PriceListIds);
        Assert.Equal(1, config.PriceListIds.First());
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_NullSalesAreasResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync((string)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetSalesAreaConfigurations(traceId));
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_EmptySalesAreasResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetSalesAreaConfigurations(traceId));
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_NullPriceListsResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        var salesAreas = new List<SalesAreaResponse> { new() { SalesAreaId = 1 } };

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(salesAreas));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceLists, It.IsAny<string>()))
            .ReturnsAsync((string)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetSalesAreaConfigurations(traceId));
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_EmptyPriceListsResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        var salesAreas = new List<SalesAreaResponse> { new() { SalesAreaId = 1 } };

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(salesAreas));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceLists, It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetSalesAreaConfigurations(traceId));
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_NullCurrenciesResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        var salesAreas = new List<SalesAreaResponse> { new() { SalesAreaId = 1 } };
        var priceLists = new List<PriceListClientResponse> { new() { PriceListId = 1 } };

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(salesAreas));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceLists, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(priceLists));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Core.Currencies, It.IsAny<string>()))
            .ReturnsAsync((string)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetSalesAreaConfigurations(traceId));
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_EmptyCurrenciesResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        var salesAreas = new List<SalesAreaResponse> { new() { SalesAreaId = 1 } };
        var priceLists = new List<PriceListClientResponse> { new() { PriceListId = 1 } };

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(salesAreas));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceLists, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(priceLists));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Core.Currencies, It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetSalesAreaConfigurations(traceId));
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_ApiException_ThrowsInvalidOperationException()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("API Error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetSalesAreaConfigurations(traceId));
        Assert.Equal("API Error", exception.Message);

        // Verify error logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}