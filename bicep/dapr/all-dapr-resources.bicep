param resourceGroupName string
param targetLocation string
param cosmosDbAccountName string
param cosmosDbDatabase string
param cosmosDbDaprActorStateContainer string
param cosmosDbDaprGlobalStateContainer string

param daprServiceBusNamespace string
param daprServicebusPubSubTopicName string

resource daprCosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2022-08-15' = {
  name: cosmosDbAccountName
  kind: 'GlobalDocumentDB'
  location:targetLocation

  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: targetLocation
      }
    ]
  }

  resource daprCosmosDbDatabase 'sqlDatabases@2022-08-15' = {
    name: cosmosDbDatabase
    properties: {
      resource: {
        id: cosmosDbDatabase
      }
    }

    resource daprCosmosActorStateDbContainer 'containers@2022-08-15' = {
      name: cosmosDbDaprActorStateContainer
      properties: {
        options:{
          autoscaleSettings:{
            maxThroughput: 1000
          }
        }
        resource: {
          id: cosmosDbDaprActorStateContainer
          partitionKey:{
            kind: 'Hash'
            paths: [ '/partitionKey' ]
          }
          indexingPolicy:{
            automatic: true
          }
        }
      }
    }

    resource daprCosmosGlobalStateDbContainer 'containers@2022-08-15' = {
      name: cosmosDbDaprGlobalStateContainer
      properties: {
        options:{
          autoscaleSettings:{
            maxThroughput: 1000
          }
        }
        resource: {
          id: cosmosDbDaprGlobalStateContainer
          partitionKey:{
            kind: 'Hash'
            paths: [ '/partitionKey' ]
          }
          indexingPolicy:{
            automatic: true
          }
        }
      }
    }
  }
}


resource daprServicebusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: daprServiceBusNamespace
  location: targetLocation
  sku:{
    name: 'Standard'
    tier: 'Standard'
  }

  resource daprServicebusPubSubTopic 'topics@2022-10-01-preview' = {
    name: daprServicebusPubSubTopicName

    resource daprServicebusPubSubTopicSubscription 'subscriptions@2022-10-01-preview' = {
      name: '${daprServicebusPubSubTopicName}-subscription'
      properties:{
        maxDeliveryCount: 10
      }
    }
  }
}
