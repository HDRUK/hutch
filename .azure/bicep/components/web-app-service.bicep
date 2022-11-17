param appHostnames array = []
param appName string
param aspName string
param logAnalyticsWorkspaceName string
param connectionStrings array = []
param tags object = {}

param location string = resourceGroup().location

resource asp 'Microsoft.Web/serverfarms@2020-10-01' existing = {
  name: aspName
}

resource laWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logAnalyticsWorkspaceName
}

// https://docs.microsoft.com/en-us/azure/templates/microsoft.insights/2020-02-02/components
resource appinsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${appName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    IngestionMode: 'LogAnalytics' // "new" AppInsights
    RetentionInDays: 90
    WorkspaceResourceId: laWorkspace.id
  }
  tags: union({
    Source: 'Bicep'
  }, tags)
}

// https://docs.microsoft.com/en-us/azure/templates/microsoft.web/sites
resource app 'Microsoft.Web/sites@2020-10-01' = {
  name: appName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: asp.id
    enabled: true
    siteConfig: {
      connectionStrings: connectionStrings
      netFrameworkVersion: 'v6.0'
      linuxFxVersion: 'DOTNETCORE|6.0'
      requestTracingEnabled: true
      httpLoggingEnabled: true
      use32BitWorkerProcess: true
      alwaysOn: true
      managedPipelineMode: 'Integrated'
      virtualApplications: [
        {
          virtualPath: '/'
          physicalPath: 'site\\wwwroot'
          preloadEnabled: true
        }
      ]
      http20Enabled: true
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }

  tags: union({
    Source: 'Bicep'
  }, tags)
}

@batchSize(1) // Hostnames need to be done serially
resource hostnameBinding 'Microsoft.Web/sites/hostNameBindings@2021-02-01' = [for hostname in appHostnames: {
  name: hostname
  parent: app
  properties: {
    siteName: appName
    sslState: 'Disabled'
    hostNameType: 'Verified'
  }
}]

output name string = appName
output tenantId string = app.identity.tenantId
output principalId string = app.identity.principalId
output aspId string = asp.id
output appInsightsName string = appinsights.name
