param location string = resourceGroup().location

var env = 'uat'

var appHostnames = [
  // TODO when a real prod environment is added, we should subdomain UAT
  'sargassure.com'
  'sargassure.net'
  'sargassure.co.uk'
  // 'sargassure.org'
]

var aspName = 'nonprod-asp' // shared by all non prod apps
var apiBaseUrl = 'https://sargassure.com/api'

resource kv 'Microsoft.KeyVault/vaults@2020-04-01-preview' existing = {
  name: 'sargassure-${env}-kv'
}

module web 'web.bicep' = {
  name: 'sargassure-web-${uniqueString(env)}'
  params: {
    location: location
    env: env
    aspName: aspName
    keyVaultName: kv.name
    appHostnames: appHostnames
    appSettings: {
      // AspNetCore__Environment: env // TODO specify env when a real prod environment is added
    }
  }
}

// Worker Functions App
module worker 'worker.bicep' = {
  name: 'sargassure-worker-${uniqueString(env)}'
  params: {
    location: location
    env: env
    keyVaultName: kv.name
    aspName: aspName
    webApiBaseUrl: apiBaseUrl
  }
}
