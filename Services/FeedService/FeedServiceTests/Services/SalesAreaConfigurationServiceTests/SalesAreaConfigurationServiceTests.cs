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
    private readonly Mock<IOptionsMonitor<NorceBaseModuleOptions>> _norceOptionsMonitorMock;
    private readonly Mock<IOptionsMonitor<BaseModuleOptions>> _baseOptionsMonitorMock;
    private readonly SalesAreaConfigurationService _service;
    private readonly BaseModuleOptions _baseOptions;

    public SalesAreaConfigurationServiceTests()
    {
        _loggerMock = new Mock<ILogger<SalesAreaConfigurationService>>();
        _norceClientMock = new Mock<INorceClient>();
        _queryClientMock = new Mock<INorceQueryClient>();
        _norceOptionsMonitorMock = new Mock<IOptionsMonitor<NorceBaseModuleOptions>>();
        _baseOptionsMonitorMock = new Mock<IOptionsMonitor<BaseModuleOptions>>();

        _baseOptions = new BaseModuleOptions
        {
            IncludedSalesAreaIds = "1|2" // This will populate IncludedSalesAreaIdsList with [1,2]
        };
        _baseOptionsMonitorMock.Setup(x => x.CurrentValue).Returns(_baseOptions);

        // Setup Query property to return the mock query client
        _norceClientMock.Setup(x => x.Query).Returns(_queryClientMock.Object);

        _service = new SalesAreaConfigurationService(
            _loggerMock.Object,
            _norceClientMock.Object,
            _norceOptionsMonitorMock.Object,
            _baseOptionsMonitorMock.Object
        );
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_Success_ReturnsSalesAreaConfigurations()
    {
        // Arrange
        var traceId = Guid.NewGuid();
       

        var salesAreas = new OdataWrapperResponse<SalesAreaResponse>
        {
            Context = "odata",
            Value = new List<SalesAreaResponse>
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
            }
        };

        var priceLists = new OdataWrapperResponse<PriceListClientResponse>
        {
            Context = "odata",
            Value = new List<PriceListClientResponse>
            {
                new()
                {
                    PriceListId = 1,
                    SalesAreaId = 1,
                    CurrencyId = 1,
                    IsActive = true,
                    PriceList = new PriceListResponse { IsActive = true }
                }
            }
        };
        var currencies = new OdataWrapperResponse<CurrenciesResponse>
        {
            Context = "odata",
            Value = new List<CurrenciesResponse>
            {
                new()
                {
                    Id = 1,
                    Code = "SEK",
                    IsActive = true
                }
            }
        };

        var salesAreasJson = JsonSerializer.Serialize(salesAreas);
        var priceListsJson = JsonSerializer.Serialize(priceLists);
        var currenciesJson = JsonSerializer.Serialize(currencies);

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(salesAreasJson);

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceListsIncPriceList, It.IsAny<string>()))
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
        

        var salesAreas = new OdataWrapperResponse<SalesAreaResponse>
        {
            Context = "odata",
            Value = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "SE" },
            }
        };


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

        var salesAreas = new OdataWrapperResponse<SalesAreaResponse>
        {
            Context = "odata",
            Value = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "SE" }
            }
        };


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
        

        var salesAreas = new OdataWrapperResponse<SalesAreaResponse>
        {
            Context = "odata",
            Value = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "SE" },
                new() { SalesAreaId = 2, Code = "US" }
            }
        };

        var priceLists = new OdataWrapperResponse<PriceListClientResponse>
        {
            Context = "odata",
            Value = new List<PriceListClientResponse>
            {
                new() { PriceListId = 1, SalesAreaId = 1, CurrencyId = 1, IsActive = true, PriceList = new PriceListResponse { IsActive = true } },
                new() { PriceListId = 2, SalesAreaId = 2, CurrencyId = 1, IsActive = true, PriceList = new PriceListResponse { IsActive = true } }
            }
        };
       

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
        var salesAreas = new OdataWrapperResponse<SalesAreaResponse>{Context = "odata", Value = new List<SalesAreaResponse>{new SalesAreaResponse{ SalesAreaId = 1 } } };
        var priceLists = new OdataWrapperResponse<PriceListClientResponse> {Context = "odata", Value = new List<PriceListClientResponse> {new PriceListClientResponse { PriceListId = 1 } } };

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

    [Fact]
    public async Task GetSalesAreaConfigurations_Success_ReturnsFilteredSalesAreaConfigurations()
    {
        // Arrange
        var traceId = Guid.NewGuid();

        var salesAreas = new OdataWrapperResponse<SalesAreaResponse>
        {
            Context = "odata",
            Value = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, ClientId = 1006, IsPrimary = true, Created = DateTime.UtcNow, CreatedBy = 1, Code = "SE" },
                new() { SalesAreaId = 2, ClientId = 1006, IsPrimary = false, Created = DateTime.UtcNow, CreatedBy = 1, Code = "US" },
                new() { SalesAreaId = 3, ClientId = 1006, IsPrimary = false, Created = DateTime.UtcNow, CreatedBy = 1, Code = "NO" }
            }
        };

        var priceLists = new OdataWrapperResponse<PriceListClientResponse>
        {
            Context = "odata",
            Value = new List<PriceListClientResponse>
            {
                new()
                {
                    PriceListId = 1,
                    SalesAreaId = 1,
                    CurrencyId = 1,
                    IsActive = true,
                    PriceList = new PriceListResponse { IsActive = true }
                },
                new()
                {
                    PriceListId = 2,
                    SalesAreaId = 2,
                    CurrencyId = 1,
                    IsActive = true,
                    PriceList = new PriceListResponse { IsActive = true }
                },
                new()
                {
                    PriceListId = 3,
                    SalesAreaId = 3,
                    CurrencyId = 1,
                    IsActive = true,
                    PriceList = new PriceListResponse { IsActive = true }
                }
            }
        };
        var currencies = new OdataWrapperResponse<CurrenciesResponse>
        {
            Context = "odata",
            Value = new List<CurrenciesResponse>
            {
                new()
                {
                    Id = 1,
                    Code = "SEK",
                    IsActive = true
                }
            }
        };



        var salesAreasJson = JsonSerializer.Serialize(salesAreas);
        var priceListsJson = JsonSerializer.Serialize(priceLists);
        var currenciesJson = JsonSerializer.Serialize(currencies);

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(salesAreasJson);
        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceListsIncPriceList, It.IsAny<string>()))
            .ReturnsAsync(priceListsJson);
        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Core.Currencies, It.IsAny<string>()))
            .ReturnsAsync(currenciesJson);

        // Act
        var result = await _service.GetSalesAreaConfigurations(traceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Should only contain configurations for SalesAreaIds 1 and 2
        Assert.Contains(result, c => c.SalesAreaId == 1);
        Assert.Contains(result, c => c.SalesAreaId == 2);
        Assert.DoesNotContain(result, c => c.SalesAreaId == 3);
    }

    [Fact]
    public async Task GetSalesAreaConfigurations_EmptyFilterList_ReturnsAllConfigurations()
    {
        // Arrange
        var traceId = Guid.NewGuid();
        _baseOptions.IncludedSalesAreaIds = ""; // Empty filter list

        var salesAreas = new OdataWrapperResponse<SalesAreaResponse> { Context = "odata", Value = new List<SalesAreaResponse>
            {
                new() { SalesAreaId = 1, Code = "SE" },
                new() { SalesAreaId = 2, Code = "US" }
            }
        };

        var priceLists = new OdataWrapperResponse<PriceListClientResponse> { Context = "odata", Value = new List<PriceListClientResponse>
            {
                new() { PriceListId = 1, SalesAreaId = 1, CurrencyId = 1, IsActive = true, PriceList = new PriceListResponse { IsActive = true } },
                new() { PriceListId = 2, SalesAreaId = 2, CurrencyId = 1, IsActive = true, PriceList = new PriceListResponse { IsActive = true } }
            }
        };
        var currencies = new OdataWrapperResponse<CurrenciesResponse>
        {
            Context = "odata",
            Value = new List<CurrenciesResponse>
            {
                new() { Id = 1, Code = "SEK", IsActive = true }
            }
        };


        SetupMockResponses(salesAreas, priceLists, currencies);

        // Act
        var result = await _service.GetSalesAreaConfigurations(traceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(salesAreas.Value.Count, result.Count);
    }


    private void SetupMockResponses(
        OdataWrapperResponse<SalesAreaResponse> salesAreas,
        OdataWrapperResponse<PriceListClientResponse> priceLists,
        OdataWrapperResponse<CurrenciesResponse> currencies)
    {
        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientSalesAreas, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(salesAreas));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Application.ClientPriceListsIncPriceList, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(priceLists));

        _queryClientMock.Setup(x => x.GetAsync(Endpoints.Query.Core.Currencies, It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(currencies));
    }

}