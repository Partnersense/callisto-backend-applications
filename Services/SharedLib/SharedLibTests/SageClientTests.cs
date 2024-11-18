using System.Net;
using System.Text;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Sage;
using SharedLib.Helpers.Xml;
using SharedLib.Models.Sage;
using SharedLib.Options.Models;

namespace SharedLibTests;

public class SageClientTests
{
     private readonly SageClient _sageClient;
     private readonly IXmlConverter _xmlConverter;
     private readonly HttpMessageHandler _httpMessageHandler;

    public SageClientTests()
    {
        var logger = A.Fake<ILogger<SageClient>>();
        var optionsMonitor = A.Fake<IOptionsMonitor<SageModuleOptions>>();

        _xmlConverter = A.Fake<IXmlConverter>();
        _httpMessageHandler = A.Fake<HttpMessageHandler>();
        var httpClient = new HttpClient(_httpMessageHandler);


        var httpClientFactory = A.Fake<IHttpClientFactory>();
        A.CallTo(() => httpClientFactory.CreateClient(nameof(SageClient))).Returns(httpClient);

        _sageClient = new SageClient(logger, optionsMonitor, _xmlConverter, httpClientFactory);
    }

    private void SetupFakeResponse (string responseXml, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var httpResponse = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(responseXml, Encoding.UTF8, "application/xml")
        };

        A.CallTo(_httpMessageHandler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(Task.FromResult(httpResponse));
    }

    [Fact]
    public async Task FetchProductsDetails_ReturnsNull_WhenApiResponseIsEmpty()
    {
        SetupFakeResponse("");
        var result = await _sageClient.FetchProductsDetails("test");
        Assert.Null(result);
    }

    [Fact]
    public async Task FetchProductsDetails_ReturnsCorrectData_WhenApiResponseIsValid()
    {
        var validXml = "<Envelope><Body><QueryResponse><QueryReturn><ResultXml><ProductDetail>Details</ProductDetail></ResultXml></QueryReturn></QueryResponse></Body></Envelope>";
        SetupFakeResponse(validXml);

        var queryResult = new QueryResult();
        A.CallTo(() => _xmlConverter.DeserializeXml<QueryResult>(A<string>.That.Contains("ProductDetail")))
            .Returns(queryResult);

        var result = await _sageClient.FetchProductsDetails("test");
        Assert.Null(result);
    }

    [Fact]
    public async Task FetchPrices_ReturnsNull_WhenNoPricesFound()
    {
        SetupFakeResponse("");
        var result = await _sageClient.FetchPrices();
        Assert.Null(result);
    }

    [Fact]
    public async Task FetchProductStock_ReturnsNull_WhenNoStockInformation()
    {
        SetupFakeResponse("");
        var result = await _sageClient.FetchProductStock("test");
        Assert.Empty(result);
    }

    [Fact]
    public async Task FetchProductStock_ReturnsNull_OnException()
    {
        var exception = new InvalidOperationException("Network failure");
        A.CallTo(_httpMessageHandler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Throws(exception);

        var result = await _sageClient.FetchProductStock("test");
        Assert.Empty(result);
    }
}