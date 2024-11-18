using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharedLib.Constants;
using SharedLib.Helpers.Xml;
using SharedLib.Models.Sage;
using SharedLib.Options.Models;

namespace SharedLib.Clients.Sage;
public class SageClient : ISageClient
{
    private readonly ILogger<SageClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly SageModuleOptions _config;
    private readonly IXmlConverter _xmlConverter;
    public SageClient(ILogger<SageClient> logger, IOptionsMonitor<SageModuleOptions> config, IXmlConverter xmlConverter, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _config = config.CurrentValue;
        _xmlConverter = xmlConverter;
        _httpClient = httpClientFactory.CreateClient(nameof(SageClient));
    }

    public async Task<QueryResult?> FetchProductsDetails(string warehouseCode)
    {
        try
        {
            var callContext = new SageRequestDto.CAdxCallContext
            {
                CodeLang = SageConstants.Api.Languages.En,
                PoolAlias = _config.PoolAlias,
                PoolId = string.Empty,
                RequestConfig = "<![CDATA[adxwss.optreturn=XML&adxwss.beautify=false&adxwss.trace.on=off]]>"
            };

            var objectKeys = new SageRequestDto.ArrayOfCAdxParamKeyValue
            {
                Items = new List<SageRequestDto.CAdxParamKeyValue>
                {
                    new SageRequestDto.CAdxParamKeyValue { Key = "XSITE", Value = warehouseCode }
                }
            };

            var query = new SageRequestDto.Query
            {
                CallContext = callContext,
                PublicName = SageConstants.Api.ItemEndpoint,
                ObjectKeys = objectKeys,
                ListSize = 1000
            };

            var queryXml = _xmlConverter.SerializeToXml(query);


            var xmlResponseString = await GetEntity<string>("\"\"", queryXml);
            if (string.IsNullOrWhiteSpace(xmlResponseString))
            {
                return null;
            }
            var xmlResponse = _xmlConverter.DeserializeXml<Envelope>(xmlResponseString);
            return !string.IsNullOrWhiteSpace(xmlResponse?.Body.QueryResponse.QueryReturn.ResultXml) ? _xmlConverter.DeserializeXml<QueryResult>(xmlResponse.Body.QueryResponse.QueryReturn.ResultXml) : null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get product details from ERP system");
            return null;
        }
    }

    public async Task<QueryResult?> FetchPrices(Dictionary<string, string>? paramKeyValues = null)
    {
        var resultListSize = 10000;
        try
        {
            var callContext = new SageRequestDto.CAdxCallContext
            {
                CodeLang = SageConstants.Api.Languages.En,
                PoolAlias = _config.PoolAlias,
                PoolId = string.Empty,
                RequestConfig = "<![CDATA[adxwss.optreturn=XML&adxwss.beautify=false&adxwss.trace.on=off]]>"
            };

            var objectKeys = new SageRequestDto.ArrayOfCAdxParamKeyValue
            {
                Items = paramKeyValues?.Select(kv => new SageRequestDto.CAdxParamKeyValue
                {
                    Key = kv.Key,
                    Value = kv.Value
                }).ToList() ?? []
        };

            var query = new SageRequestDto.Query
            {
                CallContext = callContext,
                PublicName = SageConstants.Api.PriceEndpoint,
                ObjectKeys = objectKeys,
                ListSize = resultListSize
            };

            var queryXml = _xmlConverter.SerializeToXml(query);


            var xmlResponseString = await GetEntity<string>("\"\"", queryXml);
            if (string.IsNullOrWhiteSpace(xmlResponseString))
            {
                return null;
            }
            var xmlResponse = _xmlConverter.DeserializeXml<Envelope>(xmlResponseString);

            var queryResult =
                _xmlConverter.DeserializeXml<QueryResult>(xmlResponse.Body.QueryResponse.QueryReturn.ResultXml);

            if (queryResult is { Size: not 0, Dimension: not 0 } && queryResult.Size >= queryResult.Dimension)
            {
                _logger.LogError("Retrieved the maximum number of elements for the query: {MaxCount}. Adjust the request or fine tune the query as needed to reduce the result.", resultListSize);
            }

            return !string.IsNullOrWhiteSpace(xmlResponse.Body.QueryResponse.QueryReturn.ResultXml) ? queryResult : null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get product details from ERP system");
            return null;
        }
    }

    public async Task<List<SageStockResultLine>> FetchProductStock(string warehouseCode)
    {
        var retVal = new List<SageStockResultLine>();
        var page = 1;
        var totalPages = 1;
        do
        {
            var result = await FetchStockPage(warehouseCode, page);
            if (result == null)
            {
                _logger.LogError("Failed to get stock details from SageX3. Warehouse: {Warehouse} Page: {Page}", warehouseCode, page); 
                break;
            }

            retVal.AddRange(result.Table.Lines);
            page++;
            totalPages = int.Parse(result.Group.Fields.Find(field => field.Name.Equals(SageConstants.Fields.TotalPages))?.Value ?? "1");
        } while (page <= totalPages);

        return retVal;
    }

    private async Task<SageStockResultDto?> FetchStockPage(string warehouseCode, int page)
    {
        try
        {
            var queryXml = CreateRunRequest(SageConstants.Api.Languages.En, warehouseCode, SageConstants.Api.StockEndpoint, page);
            var xmlResponseString = await GetEntity<string>("\"\"", queryXml);

            if (string.IsNullOrWhiteSpace(xmlResponseString))
            {
                return null;
            }

            var xmlResponse = _xmlConverter.DeserializeXml<RunEnvelope>(xmlResponseString);
            var resultXml = xmlResponse?.Body.Response.Return.ResultXml;

            return !string.IsNullOrWhiteSpace(resultXml)
                ? _xmlConverter.DeserializeXml<SageStockResultDto>(resultXml)
                : null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to fetch stock page {Page} from ERP system", page);
            return null;
        }
    }

    public async Task<T?> GetEntity<T>(string soapAction, string soapBody)
    {
        try
        {
            var soapEnvelope = string.Format(SageConstants.SageClientConstants.SoapEnvelope, soapBody);
            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Headers.Add("SOAPaction", soapAction);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"SOAP request failed: {response.ReasonPhrase}. Error: {errorContent}");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
            {
                return (T)(object)responseContent;
            }
            return !string.IsNullOrWhiteSpace(responseContent) ? JsonConvert.DeserializeObject<T>(responseContent) : default;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to fetch {Entity}", typeof(T).Name);
            return default;
        }
    }

    public string CreateRunRequest(string langCode, string warehouseCode, string apiCode, int page, bool? beautify = false)
    {
        return @$"<wss:run soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                                <callContext xsi:type=""wss:CAdxCallContext"">
                                    <codeLang xsi:type=""xsd:string"">{langCode}</codeLang>
                                    <poolAlias xsi:type=""xsd:string"">{_config.PoolAlias}</poolAlias>
                                    <poolId xsi:type=""xsd:string""></poolId>
                                    <requestConfig xsi:type=""xsd:string""><![CDATA[adxwss.optreturn=XML&adxwss.beautify={beautify}&adxwss.trace.on=off]]></requestConfig>
                                </callContext>
                                <publicName xsi:type=""xsd:string"">{apiCode}</publicName>
                                      <inputXml xsi:type=""xsd:string"">
                                           <![CDATA[<?xml version=""1.0"" encoding=""UTF-8""?>
		                                    <PARAM>
		                                        <GRP ID=""GRP1"">
			                                        <FLD NAME=""PAGE"" TYPE=""Char"">{page}</FLD>
			                                        <FLD NAME=""SITE"" TYPE=""Char"">{warehouseCode}</FLD>
		                                        </GRP>	
		                                    </PARAM>]]>
		                                </inputXml>
                            </wss:run>";

    }
}
