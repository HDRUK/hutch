param location string = resourceGroup().location

var env = 'shared'

module la './components/log-analytics-workspace.bicep' = {
  name: 'la-ws-${uniqueString(env)}'
  params: {
    location: location
    logAnalyticsWorkspaceName: 'sargassure-la-ws'
    tags: {
      Environment: env
    }
  }
}
