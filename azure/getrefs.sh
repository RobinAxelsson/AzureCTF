#!/usr/bin/env bash
for VAR in "$@"; do
    echo "$VAR=$(python3 ./format.py "$(az keyvault secret show --id "https://linkmanagement.vault.azure.net/secrets/$VAR" | grep \"id\")")"
done
