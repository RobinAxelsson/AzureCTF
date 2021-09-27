#!/usr/bin/env bash
export accountEndpoint=$(cat .secrets/accountEndpoint.secret)
export accountKey=$(cat .secrets/accountKey.secret)
export databaseName=$(cat .secrets/databaseName.secret)
export containerName=$(cat .secrets/containerName.secret)
#cd ./src/Function
#func start --csharp
# dotnet test
# .secrets/deleteDb
