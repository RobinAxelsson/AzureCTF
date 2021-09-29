#!/usr/bin/env bash

function cleanup() {
    unset accountEndpoint
    unset accountKey
    unset databaseName
    unset containerName
    exit
}

trap cleanup SIGINT

export accountEndpoint=$(cat .secrets/accountEndpoint.secret)
export accountKey=$(cat .secrets/accountKey.secret)
export databaseName=$(cat .secrets/databaseName.secret)
export containerName=$(cat .secrets/containerName.secret)

cd ../src/Admin
func start --csharp --port 7072
