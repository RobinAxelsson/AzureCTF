#!/usr/bin/env bash

curl "http://localhost:7071/api/HttpLinks?name=bob&answer=this,is,are,they"
echo
curl "http://localhost:7071/api/HttpLinks?name=albin&answer=who,are,this,cool"
echo
curl "http://localhost:7071/api/HttpLinks?name=Steffie&answer=global,environment,variables,are,cool"
echo
curl "http://localhost:7071/api/HttpLinks?name=Alex&answer=test,my,stupid,loaf"
echo

curl "http://localhost:7072/api/HttpLinks"
echo
curl "http://localhost:7072/api/HttpLinks?cmd=delete"
