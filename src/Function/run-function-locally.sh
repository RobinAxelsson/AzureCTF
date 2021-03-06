#!/usr/bin/env bash

function cleanup() {
    unset accountEndpoint
    unset accountKey
    unset databaseName
    unset containerName
    unset answer
    unset flag
    exit
}

trap cleanup SIGINT
export accountEndpoint=$(cat .secrets/accountEndpoint.secret)
export accountKey=$(cat .secrets/accountKey.secret)
export databaseName=$(cat .secrets/databaseName.secret)
export containerName=$(cat .secrets/containerName.secret)
export secretAnswer=$(cat .secrets/secretAnswer.secret)
export flag=$(cat .secrets/flag.secret)

func start --csharp
