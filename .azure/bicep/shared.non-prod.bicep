param location string = resourceGroup().location

var env = 'non-prod'

module asp './components/app-service-plan.bicep' = {
  name: 'asp-${uniqueString(env)}'
  params: {
    aspName: 'nonprod-asp' // shared by all non prod apps
    sku: 'B1'
    location: location
    tags: {
      Environment: env
    }
  }
}
