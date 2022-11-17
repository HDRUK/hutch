param aspName string
param sku string = 'B1'
param tags object = {}

param location string = resourceGroup().location

// https://docs.microsoft.com/en-us/azure/templates/microsoft.web/serverfarms
resource asp 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: aspName
  location: location
  kind: 'linux'
  sku: {
    name: sku
  }
  properties:{
    reserved: true // linux asp always have this set to true ¯\_(ツ)_/¯
  }
  tags: union({
    Source: 'Bicep'
  }, tags)
}

output name string = asp.name
output id string = asp.id
