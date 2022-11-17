param appName string
param aspName string
param logAnalyticsWorkspaceName string
param tags object = {}

// required for linux
param runtimeStack string = 'Python|3.9'

param connectionStrings array = []

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
resource functionApp 'Microsoft.Web/sites@2020-10-01' = {
  name: appName
  location: location
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: asp.id
    siteConfig: {
      connectionStrings: connectionStrings
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      linuxFxVersion: runtimeStack
    }
    httpsOnly: true
  }
  tags: union({
    Source: 'Bicep'
  }, tags)
}

output name string = functionApp.name
output tenantId string = functionApp.identity.tenantId
output principalId string = functionApp.identity.principalId
output appInsightsName string = appinsights.name
