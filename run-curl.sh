#!/usr/bin/env bash

pushd .secrets 1>/dev/null
bash curl.sh
popd 1>/dev/null
