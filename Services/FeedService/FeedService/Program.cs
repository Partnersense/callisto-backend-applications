using FeedService.Jobs;
using SharedLib.Configuration.Norce;
using SharedLib.Middleware;
using SharedLib.Options;
using SharedLib.Configuration;
using SharedLib.Services;
using SharedLib.Options.Models;
using SharedLib.Jobs;
using SharedLib.Options.Models.Logging;

var builder = WebApplication.CreateBuilder(args);

#region App Configuration

builder.Configuration.Sources.Clear();
builder.UseCustomAppConfiguration("FEEDBUILDER");
builder.Services.AddOptions();
builder.Services.AddSingleton<ConfigurationMonitorService>();

#endregion


#region Options

builder.Services.UseNorceOptionsRegistry();
builder.Services.UseStorageOptionsRegistry();
builder.Services.UseHangfireOptionsRegistry();
builder.Services.AddOptions<NorceProductFeedModuleOptions>()
    .BindConfiguration("Options:NorceProductFeedConfiguration")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<BaseModuleOptions>()
    .BindConfiguration("Options:Base")
    .ValidateDataAnnotations()
    .ValidateOnStart();

#endregion

#region Configuration

builder.Services.ConfigureDefaultNorceClients();

#endregion

#region Logging

var loggingOptions = builder.Configuration.GetSection("Options:ElkLoggingConfiguration").Get<ElasticLoggingModuleOptions>();
var azureLoggingOptions = builder.Configuration.GetSection("Options:AzureBlobLoggingConfiguration").Get<AzureBlobLoggingModuleOptions>();

builder.AddDefaultLogging(loggingOptions!, azureLoggingOptions!);

#endregion

#region Hangfire Server

// Get Hangfire options
var hangfireOptions = builder.Configuration.GetSection("Options:HangfireConfiguration").Get<HangfireModuleOptions>();

// Run Hangfire if we are not in development or if flag is set
if (!builder.Environment.IsDevelopment() || hangfireOptions!.RunHangfireLocally)
{
    builder.Services.AddDefaultHangfireServer(hangfireOptions!.ServerName);
}

#endregion

#region Services

builder.Services.AddTransient<IStorageService, StorageService>();

builder.Services.AddTransient<IJob, FeedBuilder>();
builder.Services.AddHostedService<Worker>();

#endregion

var app = builder.Build();

#region Hangfire Dashboard

// Use Hangfire locally if needed
if (!builder.Environment.IsDevelopment() || hangfireOptions!.RunHangfireLocally)
{
    app.AddDefaultHangfireDashboard(hangfireOptions);
}

#endregion
var useAzureAppSettings = bool.TryParse(Environment.GetEnvironmentVariable("USE_AZURE_APP_CONFIGURATION"), out var result) && result;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    if (useAzureAppSettings)
    {
        app.UseAzureAppConfiguration();
    }

    // Initialize the configuration monitor service
    app.Services.GetRequiredService<ConfigurationMonitorService>();
}

await app.RunAsync();

