# Callisto backend applications project

**Table of Contents**

<!-- TOC start --><!--ts--><!--te-->

<!-- TOC end --><!--ts--><!--te-->

## Pull Requests 👀

All code in the projects must go through a code review to maintain a certain code standard, ensure that no "harmful" code is introduced and that all introduced environment variables have been saved in the correct place. The projects must be set up to need 2 approvals from 2 different people before PR can be merged.

## Environment Variables and Configuration Management 🔌

### Overview

Our .NET services utilizes Azure App Configuration to manage environment variables and application settings. This setup allows for flexible configuration management, enabling developers to easily switch between local and remote configurations during development and deployment.

### Configuration Files

The service uses the following configuration files:

- `launchSettings.json`: The startup configuration file for local development (found in Bitwarden).
- `appsettings.json`: The default configuration file for local development.

### Environment Variables

The application behavior is controlled by some key environment variables:

1. **`USE_AZURE_APP_CONFIGURATION`**

   - **Description:** Determines whether the application should fetch settings from Azure App Configuration.
   - **Values:**
     - `true`: Fetch settings from Azure App Configuration.
     - `false`: Use local configuration files.

2. **`USE_REMOTE_ENVIRONMENT_TYPE`**

   - **Description:** Specifies the environment type to fetch settings for (e.g., Stage, Production).
   - **Values:** Environment type string (e.g., `Stage`, `Production`).

3. **`AZURE_APP_CONFIGURATION_CONNECTION`**

   - **Description:** Specifies the connection string for the SHARED app configuration. These are configurations for functionality that are typically shared between services and are most likely the same every time.

4. **`<ServiceName>_SERVICE_AZURE_APP_CONFIGURATION_CONNECTION`**
   - **Description:** Specifies the connection string for the SERVICE SPECIFIC app configuration. These are configurations that are specific for the service and not used in other services.

**IMPORTANT NOTE: The configuration is set in a specific order: Shared > Service specific > Local. This means that if the same value are provided in multiple app configurations, the local value will override the service specific value, and the service specific value will override the shared value**

### Local Development

During local development, you can add or override settings in the local `appsettings.json` file. Here’s how you can set up your local environment:

1. **Add Environment Variables Locally:**

   - Open the `appsettings.json` file and add any required settings:
     ```json
     {
       "MySetting": "localValue"
     }
     ```

2. **Configure Environment Variables:**
   - In your local environment, set the following variables:
     ```sh
     USE_AZURE_APP_CONFIGURATION=false or true
     USE_REMOTE_ENVIRONMENT_TYPE=Stage
     ```

### Using Azure App Configuration

When deploying or testing with Azure App Configuration, follow these steps:

1. **Set Environment Variables:**

   - Configure the application to use Azure App Configuration:
     ```sh
     USE_AZURE_APP_CONFIGURATION=true
     USE_REMOTE_ENVIRONMENT_TYPE=Stage
     ```

2. **Update Azure App Configuration:**
   - Ensure all environment variables and settings in `appsettings.json` are added to Azure App Configuration.
   - Use the Azure portal, CLI, or scripts to add/update settings.

### Adding/Updating Configuration in Azure

When creating a pull request (PR), always add or update the necessary environment variables in Azure App Configuration:

1. **Open Azure App Configuration in the Azure Portal.**
2. **Navigate to the Configuration Explorer.**
3. **Add or Update Settings:**
   - Add new settings that were introduced in your PR.
   - Update existing settings as needed.

#### Updating configuration without redeployment

You can also update the configuration values without redeploying the service, using the `Sentinel` key:

1. **Open Azure App Configuration in the Azure Portal.**
2. **Navigate to the Configuration Explorer.**
3. **Add or Update Settings:**
   - Update existing settings as needed.
4. **Update the Key/Value `Sentinel` to an updated value**

### Example Configuration

Here's an example using the SharedLib of how to set up your `Program.cs` to use Azure App Configuration:

In program.cs:

```csharp
// Custom Application Configuration Extension
builder.Configuration.Sources.Clear();
builder.UseCustomAppConfiguration();
builder.Services.AddOptions();

// Register the configuration monitor service
builder.Services.AddSingleton<ConfigurationMonitorService>();

// Add service specific options
```

## Deployment

The deployments are don via github workflows. To add a deployment project, check out the "deploy-stage-<project-name>" and "deploy-prod-<project-name>" workflows.

### Stage

Deployments to Stage are done automatically when merging code into main. Deployments will be done for every project that has changed in their specific project folders. If there has been no changes in the project folder, but a project still has to be deployed, it is currently done manually by running the project specific workflow.
![image](https://github.com/user-attachments/assets/09e02a77-be1b-46a3-a5ad-c3949dfa6407)

This workflow will:

- Deploy to the project specific Azure Web App
- Create a pre-release in github

### Production

To release the project to production, the pre-release in the previous step has to be promoted to "latest". This is done in the GUI:

![image](https://github.com/user-attachments/assets/18825aa1-ac5e-4ab5-94ff-a67acb200bc6)

![image](https://github.com/user-attachments/assets/e691d858-0c26-41ef-a896-71d32f1fdbf1)

![image](https://github.com/user-attachments/assets/b391575d-7691-4d7b-b179-1533a2590a6b)

## Branching strategies

## Github workflow

There are two workflows that is usually setup for each service:

### CI - NOTE: Currently runs tests for all projects

When a project has been created, it has to add a ci-trigger workflow file (see templates (TODO: link to templates)). The trigger specifies which file changes that should trigger the CI-pipeline, and if there are any changes, the pipeline will try to build and test the project using `dotnet` commands.

### CD - NEEDS TESTING

When a project has to be built to an image for deployment, it has to add a build-worker workflow file (see templates (TODO: link to templates)). The trigger specifies which folder the `Dockerfile` exists in, aswell as the image name, and builds the image with an appropriate tag.
