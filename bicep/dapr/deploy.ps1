# To run:  .\bicep\dapr\deploy.ps1

az deployment group create --name 'dapr-bicep-deployment' `
--resource-group 'dapr-localdev-rg' `
--template-file '.\bicep\dapr\all-dapr-resources.bicep' `
--parameters `
resourceGroupName="dapr-localdev-rg" `
targetLocation="uksouth" `
cosmosDbAccountName="mydaprcosmos5afa2f1ec54d" `
cosmosDbDatabase="daprdb" `
cosmosDbDaprActorStateContainer="actorstate" `
daprServiceBusNamespace="daprsbns2bf609dbd7b4" `
daprServicebusPubSubTopicName="orders"

