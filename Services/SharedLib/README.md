# SharedLib
Shared library that contains configuration, clients and shared functionality for systems that is used by Strandberg.

## Table of Contents
<!-- TOC start --><!--ts--><!--te-->

<!-- TOC end --><!--ts--><!--te-->

### People
Add people working on this project

#### External dependencies
- Norce
- Sage X3
- Database
- Azure

### App Configuration
This is how you set up your service to use azure app settings. <p>
**This setup will most likely be required for almost every service, and most of the functionality in this library will depend on these configurations**</p>
```c#
builder.Configuration.Sources.Clear();
builder.UseCustomAppConfiguration();
builder.Services.AddOptions();
```

### Monitoring App Configuration
```c#
builder.Services.AddSingleton<ConfigurationMonitorService>();
// After building the app
app.Services.GetRequiredService<ConfigurationMonitorService>();
```

### Using the feature registry
```c#
builder.Services.AddFeatureManagement();
builder.Services.AddSingleton<IFeatureRegistry, FeatureRegistry>();
// After building the app
app.Services.GetRequiredService<IFeatureRegistry>().ListEnabledFeaturesAsync().Wait();
```

### Norce
To use the Norce clients, add this to your startup configuration:
```c#
builder.Services.UseNorceOptionsRegistry();
builder.Services.ConfigureDefaultNorceClients();
```

The Norce client is built as a base client, with the different clients as extensions. It can then be used to either call predefined methods specific to the client, or use a generic HTTP request. Usage: BaseClient.<Host>.<Method>

Example: <br/>
```c#
norceClient.<b>Connect.ImportProducts(products, ImportBatchSize);
norceClient.ProductFeed.FetchProductExport("channelkey")
norceClient.Api.GetAsync("query")
```


### Sage
To use the Sage client, add this to your startup configuration:
```c#
builder.Services.UseSageClientOptionsRegistry();
var sageClientOptions = builder.Configuration.GetSection("Options:SageClientConfiguration").Get<SageModuleOptions>();
builder.Services.AddHttpClient<ISageClient, SageClient>(nameof(SageClient),client =>
{
    client.BaseAddress = new Uri(sageClientOptions!.BaseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Basic {sageClientOptions.Token}");
});
builder.Services.AddTransient<ISageClient, SageClient>();
```

### Hangfire
To use hangfire, add this to your startup configuration:
```c#
builder.Services.UseHangfireOptionsRegistry();

var hangfireOptions = builder.Configuration.GetSection("Options:HangfireConfiguration").Get<HangfireModuleOptions>();
builder.Services.AddDefaultHangfireServer(hangfireOptions!.ServerName);

var app = builder.Build();

// After building the app
app.AddDefaultHangfireDashboard(hangfireOptions);
```
You can then use the Job integration to add multiple jobs to run with hangfire.
Example from Product import service:
```c#
builder.Services.AddTransient<IJob, MasterDataImport>();
builder.Services.AddTransient<IJob, FastChangingDataImport>();
builder.Services.AddHostedService<Worker>();
```

### Storage service (Azure)
To use the storage service, add this to your startup configuration:
```c#
builder.Services.UseStorageOptionsRegistry();
builder.Services.AddTransient<IStorageService, StorageService>();
```


## External documentation

[Confluence](https://link.com)

[Postman](https://link.com)