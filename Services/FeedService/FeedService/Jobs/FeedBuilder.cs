// ---------------------------------------------------------------------------
// This file contains the FeedBuilder class. It fetches a full feed from
// Norce, maps the feed to different markets and then builds a unified feed
// where a product contains the property values of all different markets in
// the format: "PropertyName_Market": <PropertyValue>. It then sends the file
// to an Azure Storage blob to be used by external systems.
// ---------------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using FeedService.Constants;
using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Internal.MethodOptions;
using FeedService.Domain.Models;
using FeedService.Domain.Norce;
using FeedService.Domain.Validation;
using FeedService.Helpers;
using FeedService.Services.CultureConfigurationServices;
using FeedService.Services.CultureFeedGenerationServices;
using FeedService.Services.PriceFeedGenerationServices;
using FeedService.Services.SalesAreaConfigurationServices;
using FeedService.Services.StorageServices;
using Hangfire;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using SharedLib.Clients.Norce;
using SharedLib.Constants;
using SharedLib.Constants.NorceConstants;
using SharedLib.Jobs;
using SharedLib.Logging.Enums;
using SharedLib.Options.Models;
using SharedLib.Services;
using SharedLib.Util;
using NorceConstants = SharedLib.Constants.NorceConstants.NorceConstants;

namespace FeedService.Jobs;


/// <summary>
/// Service responsible for building and managing product feeds by integrating data from multiple sources.
/// The FeedBuilder fetches product data from Norce, processes it for different markets and cultures,
/// combines the data into a unified feed, and uploads it to chosen Storage.
/// </summary>
/// <remarks>
/// The FeedBuilder performs the following key operations:
/// 1. Fetches configuration and market data from norce
/// 2. Processes product data for different cultures and sales areas
/// 3. Combines the data into a unified feed where each product contains properties for all markets and sales areas
/// 4. Uploads the final feed to Storage
/// 
/// Feed Property Format:
/// - Culture-specific properties: "{PropertyName}_{CultureCode}" (e.g., "Title_en-US")
/// - Sales area properties: "{PropertyName}_{SalesAreaCode}" (e.g., "Price_SE")
/// </remarks>
public class FeedBuilder(
    ILogger<FeedBuilder> logger,
    IConfigurationRefresher configurationRefresher,
    ICultureConfigurationService cultureConfigurationService,
    ISalesAreaConfigurationService salesAreaConfigurationService,
    ICultureFeedGenerationService cultureFeedGenerationService,
    IPriceFeedGenerationService priceFeedGenerationService,
    IStorageUploadService storageUploadService) : JobBase<FeedBuilder>(logger)
{

    /// <summary>
    /// Executes the feed building process.
    /// </summary>
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation(
                "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameter Action: {action}",
                traceId,
                nameof(Execute),
                nameof(LoggingTypes.CheckpointLog),
                nameof(Execute),
                "feed building process",
                nameof(MethodActionLogTypes.Starting)
            );

            // Refresh app configuration
            await configurationRefresher.TryRefreshAsync(cancellationToken);

            // Get configurations
            var cultures = await cultureConfigurationService.GetCultureConfigurations(traceId);
            var salesAreas = await salesAreaConfigurationService.GetSalesAreaConfigurations(traceId);

            if (!cultures.Any() || !salesAreas.Any())
            {
                throw new InvalidOperationException("Missing required configurations for feed generation");
            }

            // Generate SalesArea specific
            var salesAreaSpecificFeed = await priceFeedGenerationService.GenerateFeedWithPrices(salesAreas, traceId);

            //Generate Culture specific feed
            var cultureSpecificFeed = await cultureFeedGenerationService.GenerateFeedWithCultures(cultures, cancellationToken, traceId);

            // Combine and upload feed
            var combinedFeed = ProductFeedUnifierHelper.CombineMarketProducts(salesAreaSpecificFeed, cultureSpecificFeed, logger, traceId);

            if (!combinedFeed.Any())
            {
                throw new InvalidOperationException("No products found in combined feed");
            }

            //Upload feed to choosen storage, the one impemented currently is azure, but can be changed easily
            var uploadSuccess = await storageUploadService.UploadAsync(combinedFeed, new StorageUploadOptions { FileFormat = FileConstants.Json, FileName = traceId.ToString(), IsPublic = false, FileNamePostfix = traceId.ToString()}, traceId, cancellationToken);

            stopwatch.Stop();

            if (!uploadSuccess)
            {
                throw new InvalidOperationException("Failed to upload feed to storage");
            }

            logger.LogInformation(
                "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameter Action: {action} ExecutionTime: {executionTime}ms",
                traceId,
                nameof(Execute),
                nameof(LoggingTypes.CheckpointLog),
                nameof(Execute),
                "Feed generation completed successfully",
                nameof(MethodActionLogTypes.Completed),
                stopwatch.ElapsedMilliseconds
            );
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            logger.LogInformation(
                "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameter Action: {action}",
                traceId,
                nameof(FeedBuilder),
                nameof(LoggingTypes.CheckpointLog),
                nameof(Execute),
                "Feed generation was cancelled",
                nameof(MethodActionLogTypes.Aborted)

            );
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(
                ex,
                "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage} | Other Parameters ExecutionTime: {executionTime}ms",
                traceId,
                nameof(FeedBuilder),
                nameof(LoggingTypes.ErrorLog),
                nameof(Execute),
                ex.Source,
                ex.Message,
                ex.StackTrace,
                ex.InnerException,
                "Feed generation failed",
                stopwatch.ElapsedMilliseconds
            );
            throw;
        }

    }
}
