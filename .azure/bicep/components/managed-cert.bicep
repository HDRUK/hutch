param appName string
param hostname string
param aspId string

param location string = resourceGroup().location


// https://docs.microsoft.com/en-us/azure/templates/microsoft.web/2020-09-01/certificates
resource cert 'Microsoft.Web/certificates@2020-10-01' = {
  name: '${appName}_${hostname}'
  location: location
  properties: { // managed certs don't need password
    canonicalName: hostname
    serverFarmId: aspId
  }
}

// update the existing app's binding with the cert details
resource hostNameSsl 'Microsoft.Web/sites@2020-06-01' = {
  name: appName
  location: location
  properties: {
    hostNameSslStates: [
      {
        name: hostname
        sslState: 'SniEnabled'
        thumbprint: cert.properties.thumbprint
        toUpdate: true
      }
    ]
  }
}
