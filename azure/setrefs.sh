#!/usr/bin/env bash
stdin=$(cat)
for VAR in $stdin; do
    echo $VAR
    az functionapp config appsettings set -g "NET-YH" -n "AzureCTF-YH4" --settings $VAR
done
