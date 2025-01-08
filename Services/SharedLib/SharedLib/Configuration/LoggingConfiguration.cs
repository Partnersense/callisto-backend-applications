using Elastic.Channels;
using Elastic.CommonSchema;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SharedLib.Options.Models.Logging;
using Log = Serilog.Log;

namespace SharedLib.Configuration;

public static class LoggingConfiguration
{
    /// <summary>
    /// Extension method for adding logging to the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="elasticLoggingOptions">Required options to use for the ELK configuration.</param>
    /// <param name="blobLoggingOptions">Optional options for Azure Blob logging. If no parameter is passed, logging to blob will not be added.</param>
    public static void AddDefaultLogging(this WebApplicationBuilder builder, ElasticLoggingModuleOptions elasticLoggingOptions, AzureBlobLoggingModuleOptions? blobLoggingOptions = null)
    {
       

        // Add console logging
        var loggerConfiguration = new LoggerConfiguration()
            .WriteTo.Console();

        if (elasticLoggingOptions.RunElasticLocally)
        {
            if (elasticLoggingOptions == null) throw new ArgumentNullException(nameof(elasticLoggingOptions));

            if (string.IsNullOrEmpty(elasticLoggingOptions.ElasticCloudId))
                throw new ArgumentException("ElasticCloudId must not be null or empty", nameof(elasticLoggingOptions.ElasticCloudId));
            if (string.IsNullOrEmpty(elasticLoggingOptions.ElasticUser))
                throw new ArgumentException("ElasticUser must not be null or empty", nameof(elasticLoggingOptions.ElasticUser));
            if (string.IsNullOrEmpty(elasticLoggingOptions.ElasticPassword))
                throw new ArgumentException("ElasticPassword must not be null or empty", nameof(elasticLoggingOptions.ElasticPassword));

            // Add Elastic Cloud logging
            loggerConfiguration.WriteTo.ElasticCloud(elasticLoggingOptions.ElasticCloudId, elasticLoggingOptions.ElasticUser,
                    elasticLoggingOptions.ElasticPassword, opts =>
                    {
                        opts.MinimumLevel = LogEventLevel.Debug;
                        opts.TextFormatting = new EcsTextFormatterConfiguration<EcsDocument>();
                        opts.DataStream = new DataStreamName(elasticLoggingOptions.ApplicationName,
                            elasticLoggingOptions.ApplicationNamespace, elasticLoggingOptions.ApplicationType);
                        opts.BootstrapMethod = BootstrapMethod.Failure;
                        opts.ConfigureChannel = channelOpts =>
                        {
                            channelOpts.BufferOptions = new BufferOptions
                            {
                                ExportMaxRetries = 10
                            };
                        };
                    });
        }


        // Add Azure Blob logging only if blobLoggingOptions is provided
        if (blobLoggingOptions != null && builder.Environment.IsProduction())
        {
            if (string.IsNullOrEmpty(blobLoggingOptions.ConnectionString))
                throw new ArgumentException("ConnectionString must not be null or empty", nameof(blobLoggingOptions.ConnectionString));
            if (string.IsNullOrEmpty(blobLoggingOptions.ContainerName))
                throw new ArgumentException("ContainerName must not be null or empty", nameof(blobLoggingOptions.ContainerName));
            if (string.IsNullOrEmpty(blobLoggingOptions.FileName))
                throw new ArgumentException("FileName must not be null or empty", nameof(blobLoggingOptions.FileName));

            loggerConfiguration.WriteTo.AzureBlobStorage(blobLoggingOptions.ConnectionString, LogEventLevel.Information,
                blobLoggingOptions.ContainerName, blobLoggingOptions.FileName);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Host.UseSerilog();
        builder.Services.AddLogging();
    }
}
